// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#if SLOP
using System.Linq;
#else
using System;
using System.Reflection;
#endif
using osu.Framework.Allocation;
using osu.Framework.Graphics;
#if SLOP
using osu.Framework.Input.Handlers.Mouse;
#endif
using osu.Framework.Platform;
using osuTK;

namespace osu.Game.Rulesets.Fposu.UI
{
    /// <summary>
    /// Collects raw relative mouse movement, which (unlike the cursor position) isn't limited by the bounds of the window.
    /// </summary>
    public partial class FposuMouseInput : Component
    {
        private readonly object accumulationLock = new object();

        private Vector2 accumulatedMovement;

#if SLOP
        private MouseHandler? mouseHandler;
#else
        // the official osu!framework doesn't expose raw relative mouse movement, so it's taken from the (internal) SDL window directly.
        private IWindow? window;
        private EventInfo? mouseMoveRelativeEvent;
        private PropertyInfo? relativeMouseModeProperty;
        private Action<Vector2>? mouseMoveRelativeHandler;
#endif

        /// <summary>
        /// Whether raw relative movement is currently available. Otherwise, the camera has to follow the cursor position instead.
        /// </summary>
        public bool IsAvailable => AvailabilityOverride ?? isRelativeModeActive;

#if SLOP
        private bool isRelativeModeActive => mouseHandler?.IsRelativeModeActive == true;
#else
        private bool isRelativeModeActive => window != null && relativeMouseModeProperty?.GetValue(window) is true;
#endif

        /// <summary>
        /// Overrides <see cref="IsAvailable"/>, e.g. for testing without a mouse.
        /// </summary>
        public bool? AvailabilityOverride { get; set; }

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
#if SLOP
            mouseHandler = host.AvailableInputHandlers.OfType<MouseHandler>().FirstOrDefault();

            if (mouseHandler != null)
                mouseHandler.RawRelativeMovement += onRawRelativeMovement;
#else
            window = host.Window;

            if (window == null)
                return;

            var windowType = window.GetType();
            mouseMoveRelativeEvent = windowType.GetEvent(@"MouseMoveRelative", BindingFlags.Public | BindingFlags.Instance);
            relativeMouseModeProperty = windowType.GetProperty(@"RelativeMouseMode", BindingFlags.Public | BindingFlags.Instance);

            if (mouseMoveRelativeEvent?.EventHandlerType != typeof(Action<Vector2>))
            {
                mouseMoveRelativeEvent = null;
                return;
            }

            // like osu!framework's mouse handler, the scaling of the window is undone to get device units.
            mouseMoveRelativeHandler = delta => onRawRelativeMovement(window.Scale > 0 ? delta / window.Scale : delta);
            mouseMoveRelativeEvent.AddEventHandler(window, mouseMoveRelativeHandler);
#endif
        }

        /// <summary>
        /// Returns the movement in device units since the last call, and resets it.
        /// </summary>
        public Vector2 ConsumeMovement()
        {
            lock (accumulationLock)
            {
                Vector2 movement = accumulatedMovement;
                accumulatedMovement = Vector2.Zero;
                return movement;
            }
        }

        /// <summary>
        /// Adds movement in device units. Thread-safe.
        /// </summary>
        public void AddMovement(Vector2 movement)
        {
            lock (accumulationLock)
                accumulatedMovement += movement;
        }

        // invoked on the thread handling window events.
        private void onRawRelativeMovement(Vector2 movement) => AddMovement(movement);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

#if SLOP
            if (mouseHandler != null)
                mouseHandler.RawRelativeMovement -= onRawRelativeMovement;
#else
            if (window != null && mouseMoveRelativeEvent != null && mouseMoveRelativeHandler != null)
                mouseMoveRelativeEvent.RemoveEventHandler(window, mouseMoveRelativeHandler);
#endif
        }
    }
}

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Input.Bindings;
using osu.Game.Rulesets.Fposu.Mods;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Objects.Drawables;
using osu.Game.Rulesets.Osu.Skinning;
using osu.Game.Rulesets.Osu.UI;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Play;
using osuTK;

namespace osu.Game.Rulesets.Fposu.UI
{
    /// <summary>
    /// The osu! gameplay, displayed in first person.
    /// </summary>
    public partial class DrawableFposuRuleset : DrawableOsuRuleset, IKeyBindingHandler<GlobalAction>
    {
        // initialised before the base constructor, which creates the input manager and the playfield adjustment container.
        private readonly FposuCamera camera = new FposuCamera();

        /// <summary>
        /// The camera which the game is seen through.
        /// </summary>
        public FposuCamera Camera => camera;

        /// <summary>
        /// The screen which the game is displayed on.
        /// </summary>
        public FposuScreen Screen => mainAdjustmentContainer!.Screen;

        /// <summary>
        /// The input manager which turns the camera.
        /// </summary>
        public FposuInputManager FposuInputManager => (FposuInputManager)base.KeyBindingInputManager;

        /// <summary>
        /// The playfield adjustment container of the main playfield, which displays the game on the screen.
        /// </summary>
        public FposuPlayfieldAdjustmentContainer MainAdjustmentContainer => mainAdjustmentContainer!;

        private FposuPlayfieldAdjustmentContainer? mainAdjustmentContainer;
        private bool mainInputManagerCreated;

#if !SLOP
        [Resolved]
        private Player? player { get; set; }
#endif

        public DrawableFposuRuleset(FposuRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod>? mods = null)
            : base(ruleset, beatmap, mods)
        {
            // overlays of mods (e.g. flashlight and blinds) belong to the playfield, so they are displayed on the screen as well.
            mainAdjustmentContainer!.RulesetOverlays.Add(Overlays.CreateProxy());
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            var depthMod = Mods.OfType<FposuModDepth>().FirstOrDefault();

            if (depthMod != null)
                mainAdjustmentContainer!.DepthLayer.Apply(depthMod, Playfield);

#if !SLOP
            moveOverlaysToScreen();
#endif
        }

        protected override void Update()
        {
            base.Update();

            unclipSliderBodies();
        }

        private static readonly PropertyInfo? slider_body_path = typeof(SliderBody).GetProperty("Path", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly PropertyInfo? path_shared_data = typeof(Path).GetProperty("SharedData", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo? clip_to_root_node = typeof(BufferedDrawNodeSharedData).GetField(nameof(BufferedDrawNodeSharedData.ClipToRootNode));

        private readonly ConditionalWeakTable<Path, object> unclippedPaths = new ConditionalWeakTable<Path, object>();

        /// <summary>
        /// Slider bodies are drawn to frame buffers, which are cut off at the edges of the window.
        /// The screen extends beyond the window though, so that would cut off slider bodies on the screen.
        /// </summary>
        /// <remarks>
        /// The framework doesn't let paths choose this, so it's changed through reflection (which is ignored if it changes).
        /// </remarks>
        private void unclipSliderBodies()
        {
            if (slider_body_path == null || path_shared_data == null || clip_to_root_node == null)
                return;

            foreach (var drawable in Playfield.HitObjectContainer.AliveObjects)
            {
                if (drawable is not DrawableSlider slider || slider.SliderBody == null)
                    continue;

                if (slider_body_path.GetValue(slider.SliderBody) is not Path path || unclippedPaths.TryGetValue(path, out _))
                    continue;

                // paths are recycled by the slider body, so each one is changed once.
                if (path_shared_data.GetValue(path) is BufferedDrawNodeSharedData sharedData)
                    clip_to_root_node.SetValue(sharedData, false);

                unclippedPaths.Add(path, null!);
            }
        }

        // the input manager and the playfield adjustment container are also created for the resume overlay, which shouldn't be displayed in 3D.
        // the key bindings of osu! are used, as the gameplay is the same.
        protected override PassThroughInputManager CreateInputManager()
        {
            var osuRulesetInfo = ((FposuRuleset)Ruleset).OsuRulesetInfo;

            if (mainInputManagerCreated)
                return new ResumeOverlayInputManager(osuRulesetInfo);

            mainInputManagerCreated = true;
            return new FposuInputManager(osuRulesetInfo, camera, () => mainAdjustmentContainer?.Screen);
        }

        public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer()
        {
            if (mainAdjustmentContainer != null)
                return new OsuPlayfieldAdjustmentContainer { AlignWithStoryboard = true };

            return mainAdjustmentContainer = new FposuPlayfieldAdjustmentContainer(camera);
        }

        // resuming by clicking the cursor (osu!'s default) doesn't work well while turning the camera, so a countdown is used instead.
        protected override ResumeOverlay CreateResumeOverlay() => new DelayedResumeOverlay { Scale = new Vector2(0.65f) };

#if SLOP
        // displayed on the screen like the playfield, where the skip button can be clicked by looking at it.
        public override Container? BreakAndSkipOverlayContainer => mainAdjustmentContainer?.BreakAndSkipOverlays;

        // the storyboard (and video) is displayed on the screen as the background, instead of being hidden behind the 3D environment.
        public override Container? StoryboardContainer => mainAdjustmentContainer?.Storyboard;

        public override Container? StoryboardOverlayLayerContainer => mainAdjustmentContainer?.StoryboardOverlayLayer;
#else
        /// <summary>
        /// Official clients don't let rulesets choose where the storyboard and the break and skip overlays are displayed,
        /// so they're moved onto the screen after the player has created them.
        /// </summary>
        private void moveOverlaysToScreen()
        {
            if (player == null || mainAdjustmentContainer == null)
                return;

            // the storyboard (and video) is displayed on the screen as the background, instead of being hidden behind the 3D environment.
            if (!player.DimmableStoryboard.HasProxy)
                mainAdjustmentContainer.Storyboard.Add(player.DimmableStoryboard.CreateProxy());

            if (player.BreakOverlay.Parent is not Container overlayContainer)
                return;

            // the overlay layer of the storyboard is proxied as the first overlay (below the HUD), and only one proxy can exist per drawable.
            var storyboardOverlayLayer = overlayContainer.Children.FirstOrDefault(d => d.IsProxy);

            // displayed on the screen like the playfield, where the skip button can be clicked by looking at it.
            var breakAndSkipOverlays = overlayContainer.Children.Where(d => d is BreakOverlay or SkipOverlay).ToArray();

            if (storyboardOverlayLayer != null)
            {
                overlayContainer.Remove(storyboardOverlayLayer, false);
                mainAdjustmentContainer.StoryboardOverlayLayer.Add(storyboardOverlayLayer);
            }

            foreach (var overlay in breakAndSkipOverlays)
            {
                overlayContainer.Remove(overlay, false);
                mainAdjustmentContainer.BreakAndSkipOverlays.Add(overlay);
            }
        }
#endif

        // the skip overlays are displayed on the screen, which is inside of the ruleset's input manager, so global key bindings (e.g. the one for skipping) don't reach them.
        public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
        {
            if (e.Action != GlobalAction.SkipCutscene || mainAdjustmentContainer == null)
                return false;

            return mainAdjustmentContainer.BreakAndSkipOverlays.OfType<SkipOverlay>().Any(skipOverlay => skipOverlay.OnPressed(e));
        }

        public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
        {
        }

        /// <summary>
        /// The input manager of the resume overlay, which is a countdown and so doesn't need positional input.
        /// It's displayed above the game, so it would otherwise block clicks on the screen (e.g. on the skip button) while mouse buttons are disabled.
        /// </summary>
        private partial class ResumeOverlayInputManager : OsuInputManager
        {
            public ResumeOverlayInputManager(RulesetInfo ruleset)
                : base(ruleset)
            {
            }

            public override bool PropagatePositionalInputSubTree => false;
        }

        // the cursor is part of the 2D game displayed in 3D, so it mustn't be proxied above the HUD (which would draw it at its 2D position).
        public override GameplayCursorContainer? Cursor => null;
    }
}

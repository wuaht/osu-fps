// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Rulesets.Fposu.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Objects.Drawables;
using osu.Game.Rulesets.Osu.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Fposu.Mods
{
    /// <summary>
    /// Barrel roll, which rotates the game on the screen (along with the background) instead of the whole screen, which would turn the camera's view as well.
    /// </summary>
    /// <remarks>
    /// Unlike in osu!, the playfield isn't scaled down, as the screen extends beyond the window and so the corners of the playfield stay visible.
    /// </remarks>
    public class FposuModBarrelRoll : OsuModBarrelRoll, IApplicableToDrawableRuleset<OsuHitObject>, IApplicableToDrawableHitObject
    {
        private FposuPlayfieldAdjustmentContainer? adjustmentContainer;

        private float rotation;

        public new void ApplyToDrawableRuleset(DrawableRuleset<OsuHitObject> drawableRuleset)
        {
            adjustmentContainer = (drawableRuleset as DrawableFposuRuleset)?.MainAdjustmentContainer;
        }

        public new void ApplyToDrawableHitObject(DrawableHitObject drawable)
        {
            drawable.OnUpdate += _ =>
            {
                if (drawable is DrawableHitCircle circle)
                    circle.CirclePiece.Rotation = -rotation;
            };
        }

        // replaces the rotation of the playfield adjustment container in osu!.
        public override void Update(Playfield playfield)
        {
            rotation = (Direction.Value == RotationDirection.Counterclockwise ? -1 : 1) * 360 * (float)(playfield.Time.Current / 60000 * SpinSpeed.Value);

            if (adjustmentContainer != null)
                adjustmentContainer.ContentRotation = rotation;

            if (((OsuPlayfield)playfield).Cursor is GameplayCursorContainer cursor)
                cursor.ActiveCursor.Rotation = -rotation;
        }
    }
}

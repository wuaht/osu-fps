// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.ComponentModel;

namespace osu.Game.Rulesets.Fposu.Configuration
{
    /// <summary>
    /// The unit the sensitivity of osu!fps is entered in.
    /// </summary>
    public enum FposuSensitivityGame
    {
        [Description("cm/360")]
        CmPer360,

        [Description("Counter-Strike 2")]
        CounterStrike2,

        [Description("VALORANT")]
        Valorant,

        [Description("Apex Legends")]
        ApexLegends,

        [Description("Overwatch 2")]
        Overwatch2,

        [Description("Fortnite")]
        Fortnite,

        [Description("Rainbow Six Siege")]
        RainbowSixSiege,

        [Description("Minecraft (Java)")]
        Minecraft,
    }

    /// <summary>
    /// Converts the sensitivity of other games to cm/360 and back, using the same yaw values as mouse-sensitivity.com.
    /// </summary>
    public static class FposuSensitivity
    {
        private const double centimetres_per_inch = 2.54;

        /// <summary>
        /// The rotation in degrees per count of mouse movement at a sensitivity of 1.
        /// Minecraft isn't linear and therefore has none.
        /// </summary>
        public static double YawOf(FposuSensitivityGame game)
        {
            switch (game)
            {
                case FposuSensitivityGame.CounterStrike2:
                case FposuSensitivityGame.ApexLegends:
                    return 0.022;

                case FposuSensitivityGame.Valorant:
                    return 0.07;

                case FposuSensitivityGame.Overwatch2:
                    return 0.0066;

                case FposuSensitivityGame.Fortnite:
                    return 0.005555;

                case FposuSensitivityGame.RainbowSixSiege:
                    return 0.00572957795;

                default:
                    throw new ArgumentOutOfRangeException(nameof(game), game, null);
            }
        }

        /// <summary>
        /// The range and step of the sensitivity as it is entered in the game.
        /// </summary>
        public static (double min, double max, double precision) RangeOf(FposuSensitivityGame game)
        {
            switch (game)
            {
                case FposuSensitivityGame.CounterStrike2:
                    return (0.01, 100, 0.001);

                case FposuSensitivityGame.Valorant:
                    return (0.001, 10, 0.001);

                case FposuSensitivityGame.ApexLegends:
                    return (0.01, 30, 0.001);

                case FposuSensitivityGame.Overwatch2:
                    return (0.01, 100, 0.01);

                case FposuSensitivityGame.Fortnite:
                    return (0.01, 100, 0.01);

                case FposuSensitivityGame.RainbowSixSiege:
                    return (1, 100, 0.01);

                case FposuSensitivityGame.Minecraft:
                    return (0, 200, 0.01);

                default:
                    throw new ArgumentOutOfRangeException(nameof(game), game, null);
            }
        }

        /// <summary>
        /// Converts the sensitivity of a game to the distance in centimetres which is needed for a full turn.
        /// </summary>
        public static double ToCmPer360(FposuSensitivityGame game, double sensitivity, int dpi)
        {
            double degreesPerCount = degreesPerCountOf(game, sensitivity);

            if (degreesPerCount <= 0 || dpi <= 0)
                return 0;

            return 360 * centimetres_per_inch / (dpi * degreesPerCount);
        }

        /// <summary>
        /// Converts the distance in centimetres which is needed for a full turn to the sensitivity of a game.
        /// </summary>
        public static double FromCmPer360(FposuSensitivityGame game, double cmPer360, int dpi)
        {
            if (cmPer360 <= 0 || dpi <= 0)
                return 0;

            double degreesPerCount = 360 * centimetres_per_inch / (dpi * cmPer360);

            if (game == FposuSensitivityGame.Minecraft)
            {
                // inverse of degrees per count = (slider * 0.6 + 0.2)^3 * 8 * 0.15, with the slider being the percentage / 200.
                double f = Math.Cbrt(degreesPerCount / (8 * 0.15));
                return Math.Max(0, (f - 0.2) / 0.6) * 200;
            }

            return degreesPerCount / YawOf(game);
        }

        private static double degreesPerCountOf(FposuSensitivityGame game, double sensitivity)
        {
            if (game == FposuSensitivityGame.Minecraft)
            {
                double f = sensitivity / 200 * 0.6 + 0.2;
                return f * f * f * 8 * 0.15;
            }

            return sensitivity * YawOf(game);
        }
    }
}

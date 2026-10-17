// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Fposu.Localisation
{
    public static class FposuStrings
    {
        private const string prefix = @"osu.Game.Rulesets.Fposu.Localisation.Fposu";

        /// <summary>
        /// "osu!fps"
        /// </summary>
        public static LocalisableString Header => new TranslatableString(getKey(@"fposu_header"), @"osu!fps");

        /// <summary>
        /// "Convert your sensitivity: [mouse-sensitivity.com]({0})"
        /// </summary>
        public static LocalisableString SensitivityConverterNote(string url) => new TranslatableString(getKey(@"fposu_sensitivity_converter_note"),
            @"Convert your sensitivity: [mouse-sensitivity.com]({0})", url);

        /// <summary>
        /// "Mouse DPI"
        /// </summary>
        public static LocalisableString MouseDpi => new TranslatableString(getKey(@"fposu_mouse_dpi"), @"Mouse DPI");

        /// <summary>
        /// "The DPI your mouse is set to. Together with cm/360, this determines the sensitivity like in first person shooters."
        /// </summary>
        public static LocalisableString MouseDpiDescription => new TranslatableString(getKey(@"fposu_mouse_dpi_description"),
            @"The DPI your mouse is set to. Together with cm/360, this determines the sensitivity like in first person shooters.");

        /// <summary>
        /// "cm/360"
        /// </summary>
        public static LocalisableString CmPer360 => new TranslatableString(getKey(@"fposu_cm_per_360"), @"cm/360");

        /// <summary>
        /// "How many centimetres the mouse has to be moved for a full turn."
        /// </summary>
        public static LocalisableString CmPer360Description => new TranslatableString(getKey(@"fposu_cm_per_360_description"),
            @"How many centimetres the mouse has to be moved for a full turn.");

        /// <summary>
        /// "Field of view"
        /// </summary>
        public static LocalisableString Fov => new TranslatableString(getKey(@"fposu_fov"), @"Field of view");

        /// <summary>
        /// "The horizontal field of view in degrees."
        /// </summary>
        public static LocalisableString FovDescription => new TranslatableString(getKey(@"fposu_fov_description"), @"The horizontal field of view in degrees.");

        /// <summary>
        /// "Distance"
        /// </summary>
        public static LocalisableString Distance => new TranslatableString(getKey(@"fposu_distance"), @"Distance");

        /// <summary>
        /// "How far away the playfield is. Lower values make it appear larger."
        /// </summary>
        public static LocalisableString DistanceDescription => new TranslatableString(getKey(@"fposu_distance_description"),
            @"How far away the playfield is. Lower values make it appear larger.");

        /// <summary>
        /// "Curved playfield"
        /// </summary>
        public static LocalisableString Curved => new TranslatableString(getKey(@"fposu_curved"), @"Curved playfield");

        /// <summary>
        /// "Curves the playfield around you, so that every part of it is equally far away."
        /// </summary>
        public static LocalisableString CurvedDescription => new TranslatableString(getKey(@"fposu_curved_description"),
            @"Curves the playfield around you, so that every part of it is equally far away.");

        /// <summary>
        /// "Invert horizontal mouse movement"
        /// </summary>
        public static LocalisableString InvertHorizontal => new TranslatableString(getKey(@"fposu_invert_horizontal"), @"Invert horizontal mouse movement");

        /// <summary>
        /// "Invert vertical mouse movement"
        /// </summary>
        public static LocalisableString InvertVertical => new TranslatableString(getKey(@"fposu_invert_vertical"), @"Invert vertical mouse movement");

        /// <summary>
        /// "Absolute mode"
        /// </summary>
        public static LocalisableString AbsoluteMode => new TranslatableString(getKey(@"fposu_absolute_mode"), @"Absolute mode");

        /// <summary>
        /// "The camera looks at the cursor instead of being turned by mouse movement. Useful for tablets."
        /// </summary>
        public static LocalisableString AbsoluteModeDescription => new TranslatableString(getKey(@"fposu_absolute_mode_description"),
            @"The camera looks at the cursor instead of being turned by mouse movement. Useful for tablets.");

        /// <summary>
        /// "Background cube"
        /// </summary>
        public static LocalisableString BackgroundCube => new TranslatableString(getKey(@"fposu_background_cube"), @"Background cube");

        /// <summary>
        /// "Skybox"
        /// </summary>
        public static LocalisableString Skybox => new TranslatableString(getKey(@"fposu_skybox"), @"Skybox");

        /// <summary>
        /// "Displays a sky around you instead of the background cube. Skins can provide their own as skybox.png, a cubemap in the horizontal cross layout."
        /// </summary>
        public static LocalisableString SkyboxDescription => new TranslatableString(getKey(@"fposu_skybox_description"),
            @"Displays a sky around you instead of the background cube. Skins can provide their own as skybox.png, a cubemap in the horizontal cross layout.");

        /// <summary>
        /// "Show background"
        /// </summary>
        public static LocalisableString ShowBackground => new TranslatableString(getKey(@"fposu_show_background"), @"Show background");

        /// <summary>
        /// "Shows the beatmap background, storyboard and video on the screen. When disabled, the objects float in front of the skybox or background cube."
        /// </summary>
        public static LocalisableString ShowBackgroundDescription => new TranslatableString(getKey(@"fposu_show_background_description"),
            @"Shows the beatmap background, storyboard and video on the screen. When disabled, the objects float in front of the skybox or background cube.");

        /// <summary>
        /// "Displays a grid around you, which helps with orientation."
        /// </summary>
        public static LocalisableString BackgroundCubeDescription => new TranslatableString(getKey(@"fposu_background_cube_description"),
            @"Displays a grid around you, which helps with orientation.");

        private static string getKey(string key) => $"{prefix}:{key}";
    }
}

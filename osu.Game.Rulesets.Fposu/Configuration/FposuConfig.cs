// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Platform;
using osu.Game.Configuration;
#if SLOP
using System;
#endif

namespace osu.Game.Rulesets.Fposu.Configuration
{
    /// <summary>
    /// Provides the settings of osu!fps, wherever they are stored.
    /// </summary>
    public static class FposuConfig
    {
        /// <summary>
        /// Retrieves the bindable of an osu!fps setting.
        /// </summary>
        /// <param name="config">The configuration of the game.</param>
        /// <param name="storage">The storage of the game.</param>
        /// <param name="setting">The setting to retrieve.</param>
        public static Bindable<T> GetBindable<T>(OsuConfigManager config, Storage storage, FposuSetting setting)
        {
#if SLOP
            // the client osu!fps ships with stores its settings in the game's configuration and displays them in its own settings section.
            return config.GetBindable<T>(toOsuSetting(setting));
#else
            return FposuConfigManager.GetShared(storage).GetBindable<T>(setting);
#endif
        }

#if SLOP
        private static OsuSetting toOsuSetting(FposuSetting setting)
        {
            switch (setting)
            {
                case FposuSetting.MouseDpi:
                    return OsuSetting.SlopFposuMouseDpi;

                case FposuSetting.CmPer360:
                    return OsuSetting.SlopFposuCmPer360;

                case FposuSetting.SensitivityGame:
                    return OsuSetting.SlopFposuSensitivityGame;

                case FposuSetting.GameSensitivity:
                    return OsuSetting.SlopFposuGameSensitivity;

                case FposuSetting.Fov:
                    return OsuSetting.SlopFposuFov;

                case FposuSetting.Distance:
                    return OsuSetting.SlopFposuDistance;

                case FposuSetting.Curved:
                    return OsuSetting.SlopFposuCurved;

                case FposuSetting.InvertHorizontal:
                    return OsuSetting.SlopFposuInvertHorizontal;

                case FposuSetting.InvertVertical:
                    return OsuSetting.SlopFposuInvertVertical;

                case FposuSetting.AbsoluteMode:
                    return OsuSetting.SlopFposuAbsoluteMode;

                case FposuSetting.BackgroundCube:
                    return OsuSetting.SlopFposuBackgroundCube;

                case FposuSetting.Skybox:
                    return OsuSetting.SlopFposuSkybox;

                case FposuSetting.ShowBackground:
                    return OsuSetting.SlopFposuShowBackground;

                case FposuSetting.ScaleCursorWithDistance:
                    return OsuSetting.SlopFposuScaleCursorWithDistance;

                default:
                    throw new ArgumentOutOfRangeException(nameof(setting), setting, null);
            }
        }
#endif
    }
}

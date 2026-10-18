// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace osu.Game.Rulesets.Fposu.Configuration
{
    /// <summary>
    /// The settings of the osu!fps ruleset, stored in their own file next to the game's configuration.
    /// </summary>
    public class FposuConfigManager : IniConfigManager<FposuSetting>
    {
        protected override string Filename => @"fposu.ini";

        private static FposuConfigManager? shared;
        private static readonly object shared_lock = new object();

        /// <summary>
        /// Returns the one instance which is shared by the settings and the gameplay, so that changes apply immediately.
        /// </summary>
        public static FposuConfigManager GetShared(Storage storage)
        {
            lock (shared_lock)
            {
                // a different storage means a different game instance (e.g. in tests).
                if (shared == null || !ReferenceEquals(shared.storage, storage))
                {
                    shared?.Dispose();
                    shared = new FposuConfigManager(storage);
                }

                return shared;
            }
        }

        private readonly Storage storage;

        private FposuConfigManager(Storage storage)
            : base(storage)
        {
            this.storage = storage;
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();

            SetDefault(FposuSetting.MouseDpi, 400, 50, 32000);
            SetDefault(FposuSetting.CmPer360, 30.0, 1.0, 200.0, 0.000001);
            SetDefault(FposuSetting.SensitivityGame, (int)FposuSensitivityGame.CmPer360, 0, Enum.GetValues<FposuSensitivityGame>().Length - 1);
            SetDefault(FposuSetting.GameSensitivity, 1.0, 0.0, 200.0, 0.000001);
            SetDefault(FposuSetting.Fov, 103f, 20f, 150f, 1f);
            SetDefault(FposuSetting.Distance, 0.5f, 0.1f, 2f, 0.01f);
            SetDefault(FposuSetting.Curved, true);
            SetDefault(FposuSetting.InvertHorizontal, false);
            SetDefault(FposuSetting.InvertVertical, false);
            SetDefault(FposuSetting.AbsoluteMode, false);
            SetDefault(FposuSetting.BackgroundCube, true);
            SetDefault(FposuSetting.Skybox, true);
            SetDefault(FposuSetting.ShowBackground, true);
        }
    }

    public enum FposuSetting
    {
        MouseDpi,
        CmPer360,
        SensitivityGame,
        GameSensitivity,
        Fov,
        Distance,
        Curved,
        InvertHorizontal,
        InvertVertical,
        AbsoluteMode,
        BackgroundCube,
        Skybox,
        ShowBackground,
    }
}

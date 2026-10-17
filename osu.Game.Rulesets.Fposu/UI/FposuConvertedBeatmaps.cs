// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#if !SLOP
using osu.Framework.Bindables;
using osu.Game.Configuration;

namespace osu.Game.Rulesets.Fposu.UI
{
    /// <summary>
    /// Every beatmap of osu!fps is a converted osu! beatmap, so official clients would only list beatmaps while converted beatmaps are shown.
    /// This shows converted beatmaps while osu!fps is selected, and restores the previous setting when another ruleset is selected.
    /// </summary>
    public static class FposuConvertedBeatmaps
    {
        private static OsuConfigManager? registeredConfig;

        // bound copies, kept alive for the lifetime of the game.
        private static IBindable<RulesetInfo>? ruleset;
        private static Bindable<bool>? showConverted;

        private static bool? showConvertedBeforeSelected;

        /// <summary>
        /// Starts showing converted beatmaps while osu!fps is selected. Has to be called on the update thread.
        /// </summary>
        public static void Register(OsuConfigManager config, IBindable<RulesetInfo> gameRuleset)
        {
            if (registeredConfig == config)
                return;

            ruleset?.UnbindAll();
            showConverted?.UnbindAll();

            registeredConfig = config;
            showConvertedBeforeSelected = null;
            showConverted = config.GetBindable<bool>(OsuSetting.ShowConvertedBeatmaps);
            ruleset = gameRuleset.GetBoundCopy();
            ruleset.BindValueChanged(r => updateShowConverted(r.NewValue), true);
        }

        private static void updateShowConverted(RulesetInfo? selected)
        {
            if (showConverted == null || showConverted.Disabled)
                return;

            if (selected?.ShortName == FposuRuleset.SHORT_NAME)
            {
                showConvertedBeforeSelected ??= showConverted.Value;
                showConverted.Value = true;
            }
            else if (showConvertedBeforeSelected is bool before)
            {
                // the setting is kept if it was changed while osu!fps was selected.
                if (showConverted.Value)
                    showConverted.Value = before;

                showConvertedBeforeSelected = null;
            }
        }
    }
}
#endif

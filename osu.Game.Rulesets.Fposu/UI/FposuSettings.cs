// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Rulesets.Fposu.Configuration;
using osu.Game.Rulesets.Fposu.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays;
using osu.Game.Overlays.Settings;
using osu.Game.Online.Chat;

namespace osu.Game.Rulesets.Fposu.UI
{
    /// <summary>
    /// Settings of the osu!fps ruleset (a first person mode based on McOsu's FPoSu).
    /// </summary>
    public partial class FposuSettings : RulesetSettingsSubsection
    {
        public FposuSettings(Ruleset ruleset)
            : base(ruleset)
        {
        }

        public override IEnumerable<LocalisableString> FilterTerms => base.FilterTerms.Concat(new LocalisableString[] { "fps", "first person", "sensitivity", "fov", "mcosu" });

        [BackgroundDependencyLoader]
        private void load(Storage storage)
        {
            var config = FposuConfigManager.GetShared(storage);

            Children = new Drawable[]
            {
                new SensitivityConverterNote(),
                new SettingsItemV2(new FormSliderBar<int>
                {
                    Caption = FposuStrings.MouseDpi,
                    HintText = FposuStrings.MouseDpiDescription,
                    Current = config.GetBindable<int>(FposuSetting.MouseDpi),
                    KeyboardStep = 50,
                }),
                new SettingsItemV2(new FormSliderBar<double>
                {
                    Caption = FposuStrings.CmPer360,
                    HintText = FposuStrings.CmPer360Description,
                    Current = config.GetBindable<double>(FposuSetting.CmPer360),
                    KeyboardStep = 0.1f,
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = FposuStrings.Fov,
                    HintText = FposuStrings.FovDescription,
                    Current = config.GetBindable<float>(FposuSetting.Fov),
                    KeyboardStep = 1,
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = FposuStrings.Distance,
                    HintText = FposuStrings.DistanceDescription,
                    Current = config.GetBindable<float>(FposuSetting.Distance),
                    KeyboardStep = 0.01f,
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.Curved,
                    HintText = FposuStrings.CurvedDescription,
                    Current = config.GetBindable<bool>(FposuSetting.Curved),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.Skybox,
                    HintText = FposuStrings.SkyboxDescription,
                    Current = config.GetBindable<bool>(FposuSetting.Skybox),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.BackgroundCube,
                    HintText = FposuStrings.BackgroundCubeDescription,
                    Current = config.GetBindable<bool>(FposuSetting.BackgroundCube),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.ShowBackground,
                    HintText = FposuStrings.ShowBackgroundDescription,
                    Current = config.GetBindable<bool>(FposuSetting.ShowBackground),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.InvertHorizontal,
                    Current = config.GetBindable<bool>(FposuSetting.InvertHorizontal),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.InvertVertical,
                    Current = config.GetBindable<bool>(FposuSetting.InvertVertical),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.AbsoluteMode,
                    HintText = FposuStrings.AbsoluteModeDescription,
                    Current = config.GetBindable<bool>(FposuSetting.AbsoluteMode),
                }),
            };
        }

        /// <summary>
        /// An informational note with a clickable link to a sensitivity converter, like McOsu has.
        /// </summary>
        private partial class SensitivityConverterNote : CompositeDrawable
        {
            private const string url = @"https://www.mouse-sensitivity.com/";

            private IBindable<string> noteText = new Bindable<string>();
            private LinkFlowContainer linkContainer = null!;

            [Resolved]
            private LocalisationManager localisation { get; set; } = null!;

            [BackgroundDependencyLoader]
            private void load(OverlayColourProvider colourProvider)
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                Padding = SettingsPanel.CONTENT_PADDING;

                InternalChild = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    CornerRadius = 5,
                    CornerExponent = 2.5f,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = colourProvider.Dark2,
                        },
                        linkContainer = new LinkFlowContainer(s =>
                        {
                            s.Font = OsuFont.Style.Caption1.With(weight: FontWeight.SemiBold);
                            s.Colour = colourProvider.Content2;
                        })
                        {
                            Padding = new MarginPadding(8),
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                        },
                    }
                };

                noteText = localisation.GetLocalisedBindableString(FposuStrings.SensitivityConverterNote(url));
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                noteText.BindValueChanged(_ =>
                {
                    linkContainer.Clear();

                    var formatted = MessageFormatter.FormatText(noteText.Value);
                    linkContainer.AddLinks(formatted.Text, formatted.Links);
                }, true);
            }
        }
    }
}

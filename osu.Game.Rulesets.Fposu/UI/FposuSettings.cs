// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Configuration;
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

        private readonly Bindable<FposuSensitivityGame> game = new Bindable<FposuSensitivityGame>();
        private readonly BindableDouble gameSensitivity = new BindableDouble();

        private Bindable<int> gameSetting = null!;
        private Bindable<double> gameSensitivitySetting = null!;
        private Bindable<int> dpi = null!;
        private Bindable<double> cmPer360 = null!;

        private SettingsItemV2 cmPer360Item = null!;
        private SettingsItemV2 gameSensitivityItem = null!;

        private bool syncing;

        [BackgroundDependencyLoader]
        private void load(Storage storage, OsuConfigManager osuConfig)
        {
            Bindable<T> bind<T>(FposuSetting setting) => FposuConfig.GetBindable<T>(osuConfig, storage, setting);

            gameSetting = bind<int>(FposuSetting.SensitivityGame);
            gameSensitivitySetting = bind<double>(FposuSetting.GameSensitivity);
            dpi = bind<int>(FposuSetting.MouseDpi);
            cmPer360 = bind<double>(FposuSetting.CmPer360);

            game.Value = (FposuSensitivityGame)gameSetting.Value;
            updateGameSensitivityRange();
            gameSensitivity.Value = gameSensitivitySetting.Value;

            Children = new Drawable[]
            {
                new SettingsItemV2(new FormEnumDropdown<FposuSensitivityGame>
                {
                    Caption = FposuStrings.SensitivityGame,
                    HintText = FposuStrings.SensitivityGameDescription,
                    Current = game,
                }),
                new SensitivityConverterNote(),
                new SettingsItemV2(new FormSliderBar<int>
                {
                    Caption = FposuStrings.MouseDpi,
                    HintText = FposuStrings.MouseDpiDescription,
                    Current = dpi,
                    KeyboardStep = 50,
                }),
                cmPer360Item = new SettingsItemV2(new FormSliderBar<double>
                {
                    Caption = FposuStrings.CmPer360,
                    HintText = FposuStrings.CmPer360Description,
                    Current = cmPer360,
                    KeyboardStep = 0.1f,
#if SLOP
                    MaxDecimalDigits = 6,
#else
                    // Official lazer limits the text box to five digits, but label and tooltip can still show six.
                    LabelFormat = v => v.ToString("0.######", CultureInfo.CurrentCulture),
#endif
                }),
                gameSensitivityItem = new SettingsItemV2(new FormSliderBar<double>
                {
                    Caption = FposuStrings.GameSensitivity,
                    HintText = FposuStrings.GameSensitivityDescription,
                    Current = gameSensitivity,
                    KeyboardStep = 0.01f,
#if SLOP
                    MaxDecimalDigits = 3,
#endif
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = FposuStrings.Fov,
                    HintText = FposuStrings.FovDescription,
                    Current = bind<float>(FposuSetting.Fov),
                    KeyboardStep = 1,
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = FposuStrings.Distance,
                    HintText = FposuStrings.DistanceDescription,
                    Current = bind<float>(FposuSetting.Distance),
                    KeyboardStep = 0.01f,
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.ScaleCursorWithDistance,
                    HintText = FposuStrings.ScaleCursorWithDistanceDescription,
                    Current = bind<bool>(FposuSetting.ScaleCursorWithDistance),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.Curved,
                    HintText = FposuStrings.CurvedDescription,
                    Current = bind<bool>(FposuSetting.Curved),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.Skybox,
                    HintText = FposuStrings.SkyboxDescription,
                    Current = bind<bool>(FposuSetting.Skybox),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.BackgroundCube,
                    HintText = FposuStrings.BackgroundCubeDescription,
                    Current = bind<bool>(FposuSetting.BackgroundCube),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.ShowBackground,
                    HintText = FposuStrings.ShowBackgroundDescription,
                    Current = bind<bool>(FposuSetting.ShowBackground),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.InvertHorizontal,
                    Current = bind<bool>(FposuSetting.InvertHorizontal),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.InvertVertical,
                    Current = bind<bool>(FposuSetting.InvertVertical),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = FposuStrings.AbsoluteMode,
                    HintText = FposuStrings.AbsoluteModeDescription,
                    Current = bind<bool>(FposuSetting.AbsoluteMode),
                }),
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            game.BindValueChanged(g =>
            {
                gameSetting.Value = (int)g.NewValue;

                // cm/360 stays the source of truth while switching. Changing the range rounds and clamps the old value,
                // which must not be converted back, as it still belongs to the previous game.
                withSync(() =>
                {
                    updateGameSensitivityRange();

                    if (g.NewValue != FposuSensitivityGame.CmPer360)
                        gameSensitivity.Value = FposuSensitivity.FromCmPer360(g.NewValue, cmPer360.Value, dpi.Value);
                });

                updateVisibility();
            }, true);

            gameSensitivity.BindValueChanged(_ =>
            {
                gameSensitivitySetting.Value = gameSensitivity.Value;
                applyGameSensitivity();
            });

            dpi.BindValueChanged(_ => applyGameSensitivity());
        }

        private void applyGameSensitivity()
        {
            if (syncing || game.Value == FposuSensitivityGame.CmPer360)
                return;

            withSync(() => cmPer360.Value = FposuSensitivity.ToCmPer360(game.Value, gameSensitivity.Value, dpi.Value));
        }

        private void withSync(System.Action action)
        {
            syncing = true;

            try
            {
                action();
            }
            finally
            {
                syncing = false;
            }
        }

        private void updateGameSensitivityRange()
        {
            if (game.Value == FposuSensitivityGame.CmPer360)
                return;

            var (min, max, precision) = FposuSensitivity.RangeOf(game.Value);

            // widen first so that neither bound ever crosses the other while the range changes.
            gameSensitivity.MinValue = double.MinValue;
            gameSensitivity.MaxValue = double.MaxValue;
            gameSensitivity.Precision = precision;
            gameSensitivity.MinValue = min;
            gameSensitivity.MaxValue = max;
        }

        private void updateVisibility()
        {
            bool isCm = game.Value == FposuSensitivityGame.CmPer360;

            cmPer360Item.Alpha = isCm ? 1 : 0;
            gameSensitivityItem.Alpha = isCm ? 0 : 1;
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

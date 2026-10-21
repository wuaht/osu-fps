// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Fposu.UI;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Screens.Play;
using osu.Game.Tests.Visual;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Fposu.Tests
{
    [Category("slop")]
    public partial class TestSceneFposuAutoplaySkip : PlayerTestScene
    {
        protected override Ruleset CreatePlayerRuleset() => new FposuRuleset();

        protected override bool Autoplay => true;

        // the first object late enough to be skipped to.
        protected override IBeatmap CreateBeatmap(RulesetInfo ruleset) => new Beatmap
        {
            BeatmapInfo = { Ruleset = ruleset },
            HitObjects =
            {
                new HitCircle { StartTime = 10000, Position = new Vector2(256, 192) },
            },
        };

        private DrawableFposuRuleset drawableRuleset => (DrawableFposuRuleset)Player.DrawableRuleset;

        private FposuScreen screen => drawableRuleset.Screen;

        private double timeBefore;

        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddUntilStep("screen loaded", () => Player.DrawableRuleset is DrawableFposuRuleset && screen.IsLoaded);
            AddUntilStep("replay loaded", () => drawableRuleset.HasReplayLoaded.Value);
            AddUntilStep("skip button enabled", () => skipButton()?.Enabled.Value == true);
        }

        [Test]
        public void TestClickSkipButtonWithCursor()
        {
            AddStep("click skip button where it's displayed", () =>
            {
                timeBefore = Player.GameplayClockContainer.CurrentTime;
                InputManager.MoveMouseTo(windowPositionOfSkipButton());
                InputManager.Click(MouseButton.Left);
            });
            assertSkipped();
        }

        [Test]
        public void TestClickBesideSkipButton()
        {
            AddStep("click top left of window", () =>
            {
                timeBefore = Player.GameplayClockContainer.CurrentTime;
                InputManager.MoveMouseTo(screen.ScreenSpaceViewRectangle.TopLeft + new Vector2(5));
                InputManager.Click(MouseButton.Left);
            });
            AddWaitStep("wait", 5);
            AddAssert("not skipped", () => Player.GameplayClockContainer.CurrentTime, () => Is.LessThan(timeBefore + 2000));
        }

        [Test]
        public void TestSkipKey()
        {
            AddStep("press skip key", () =>
            {
                timeBefore = Player.GameplayClockContainer.CurrentTime;
                InputManager.Key(Key.Space);
            });
            assertSkipped();
        }

        private void assertSkipped()
        {
            // shortly after, so that the time hasn't progressed as far by itself.
            AddWaitStep("wait", 5);
            AddAssert("skipped", () => Player.GameplayClockContainer.CurrentTime, () => Is.GreaterThan(timeBefore + 2000));
        }

        /// <summary>
        /// Finds the position in the window, where the centre of the skip button is displayed on the screen.
        /// </summary>
        private Vector2 windowPositionOfSkipButton()
        {
            var view = screen.ScreenSpaceViewRectangle;
            var button = skipButton()!.ScreenSpaceDrawQuad;

            Vector2 best = view.Centre;
            float bestDistance = float.MaxValue;

            for (int x = 0; x <= 200; x++)
            {
                for (int y = 0; y <= 200; y++)
                {
                    Vector2 windowPosition = view.TopLeft + new Vector2(view.Width * x / 200, view.Height * y / 200);

                    if (screen.ToScreenSpaceFromWindow(windowPosition) is not Vector2 position)
                        continue;

                    float distance = Vector2.Distance(position, button.Centre);

                    if (distance < bestDistance)
                    {
                        best = windowPosition;
                        bestDistance = distance;
                    }
                }
            }

            return best;
        }

        private OsuClickableContainer? skipButton() => screen.ChildrenOfType<SkipOverlay>().FirstOrDefault()?.ChildrenOfType<OsuClickableContainer>().FirstOrDefault();
    }
}

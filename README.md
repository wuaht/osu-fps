<p align="center">
  <img width="500" alt="osu! logo" src="assets/osu-fps.png">
</p>

# osu!fps

Based on [McOsu](https://github.com/McKay42/McOsu)'s FPoSu.

Gameplay is the same as in osu! (beatmaps, mods, scoring and difficulty are taken from the osu! ruleset), but it's displayed on a screen in front of a first person camera. The camera is turned with the mouse like in first person shooters, and the cursor is where the centre of the view hits the screen.

This ruleset was a gamemode for [my modded osu!lazer client](https://github.com/wuaht/osu), but now it is a separate ruleset and can be used with standard osu!lazer as well.

## Installing

1. Download `osu.Game.Rulesets.Fposu.dll` from the [latest release](https://github.com/wuaht/osu-fps/releases/latest).
2. In osu!lazer, open the settings and click "Open osu! folder" (in "Maintenance").
3. Put the DLL into the `rulesets` folder and restart the game.

The settings (sensitivity, field of view etc.) can be found in the "osu!fps" section of the ruleset settings.

## Building

```shell
dotnet build osu.Game.Rulesets.Fposu
```

The ruleset is built against the official osu!lazer packages, whose version is set in `Directory.Build.props`.

When this repository is checked out as `osu-fps` next to the projects of an osu! client source tree (e.g. as a submodule), the ruleset is built against that source instead and compiled with the `SLOP` symbol. This enables integrations which the official client doesn't offer (e.g. playing osu! beatmaps without converting them, and storing the settings in the client's own configuration). On the official client, the storyboard and the skip button are moved into 3D after the player has created them instead, and converted beatmaps are shown in song select while osu!fps is selected. The tests are only built in this case.

## Licence

osu!fps is licensed under the [MIT licence](LICENCE), like osu!.

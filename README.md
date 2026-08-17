# ThermalFlaskMod

A mod for The Long Dark. Adds two separate sliders that control how fast
the Insulated Thermos cools down — indoors and outdoors.

Range: 0.10x – 5.00x, where 1.00x is vanilla behavior.
Below 1.0x means the thermos cools down slower and stays warm longer.

Supported languages: English, Russian, French, German, Spanish, Brazilian
Portuguese, Polish, Czech, Turkish, Italian, Dutch, Japanese, Korean,
Chinese (Simplified/Traditional). Setting names and descriptions switch
automatically with the game's language.

## Installation

1. Install [MelonLoader](https://melonwiki.xyz/) on the game.
2. Download `ThermalFlaskMod.zip` from [Releases](../../releases) and
   extract both files into the game's `Mods` folder:
   - `ThermalFlaskMod.dll`
   - `ModSettings.dll`
3. Launch the game. The settings are located at:
   `Settings -> Mod Settings -> Thermal Flask`

## Building from source

Requires the .NET 6 SDK and the game installed with MelonLoader (the
generated Il2Cpp assemblies must exist under
`MelonLoader/Il2CppAssemblies`).

1. Open `ThermalFlaskMod.csproj` and set your game path in
   `<TheLongDarkPath>`.
2. `dotnet build -c Release`

The built `ThermalFlaskMod.dll` is copied into `Mods` automatically if
that folder exists at the configured path.

## Known MelonLoader issue

If the MelonLoader console doesn't show up on launch and mods don't load,
this is a known issue caused by a change in Windows' DLL search order.
Renaming `version.dll` to `winhttp.dll` in the game folder fixes it.

## License

MIT — see [LICENSE](LICENSE).

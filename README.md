# SCP:  Secret Labs - SpectatorDisabler
[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.1-4baaaa.svg)](CODE_OF_CONDUCT.md)

When a player dies they are put into the tutorial role to prevent them from spectating the other players. This stops them from respawning with the knowledge of who is still alive and where they currently are.

## Installation
- Make sure you have the [EXILED framework](https://github.com/Exiled-Team/EXILED) installed
- Download the latest [release](https://github.com/zochris/SCPSL-SpectatorDisabler/releases)
- Drop the .dll into your `%appdata%\EXILED\Plugins` directory (`~/.config/EXILED/Plugins` if on Linux)
- Restart or reload your server

## Configuration

| Name                    | Type    | Default Value | Description                                              |
| ----------------------- | ------- | ------------- | -------------------------------------------------------- |
| `is_enabled`            | Boolean | true          | Indicates whether the plugin is enabled or not           |
| `debug`                 | Boolean | false         | Enables debug logging                                    |
| `tower_window_blockers` | Boolean | false         | Enables blocking the windows in the tower on the surface |
| `tower_workbench`       | Boolean | true          | Enables spawning a workbench and safe weapons in tower   |

## Recommended EXILED settings

These settings ensure the best gameplay experience when using the SpectatorDisabler.

They can be found in `%appdata%\EXILED\Configs\7777-config.yml` or `~./.config/EXILED/Configs/7777-config.yml`.

- `can_tutorial_block_scp173: false`: Prevent tutorials from blocking SCP-173
- `can_tutorial_trigger_scp096: false`: Prevent tutorials from triggering SCP-096
- `can_scp049_sense_tutorial: false`: Prevent SCP-049 from sensing tutorials

## Compatibility

This plugin uses the tutorial role to replace the spectator role. Other plugins that use the tutorial role **will not** work together with the SpectatorDisabler. Any other plugin *should* work fine.

Feel free to report plugins that cause issues.

### Known incompatibilities

- [SerpentsHand](https://github.com/Cyanox62/SerpentsHand/)
- [Spectator-Shooting-Range](https://github.com/rayzerbrain/Spectator-Shooting-Range)
- probably more to be added…

## Contributing

Refer to the [Contributing Guidelines](docs/CONTRIBUTING.md) if you want to contribute.

Everyone interacting with this repository is expected to follow the [code of conduct](CODE_OF_CONDUCT.md).


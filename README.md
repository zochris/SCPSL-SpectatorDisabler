# SCP:  Secret Labs - SpectatorDisabler
When a player dies they are put into the tutorial role to prevent them from spectating the other players. This stops them from respawning with the knowledge of who is still alive and where they currently are.

## Installation
- Make sure you have the [EXILED framework](https://github.com/galaxy119/EXILED) installed
- Download the latest [release](https://github.com/zochris/SCPSL-SpectatorDisabler/releases)
- Drop the .dll into your `%appdata%\EXILED\Plugins` directory (`~/.config/EXILED/Plugins` if on Linux)
- Restart or reload your server

## Configuration

| Name                                 | Type    | Default Value | Description                                                  |
| ------------------------------------ | ------- | ------------- | ------------------------------------------------------------ |
| `is_enabled`                         | Boolean | true          | Indicates whether the plugin is enabled or not               |
| `show_remaining_targets_message`     | Boolean | true          | Indicates wether the custom remaining targets message is shown |
| `remaining_targets_message_duration` | UShort  | 5             | How long the remaining targets message should be shown       |

## Compatibility

This plugin uses the tutorial role to replace the spectator role. Other plugins that use the tutorial role **will not** work together with the SpectatorDisabler. Any other plugin *should* work fine.

Feel free to report plugins that cause issues.

### Known incompatibilities

- [SerpentsHand](https://github.com/Cyanox62/SerpentsHand/)
- probably more to be added…
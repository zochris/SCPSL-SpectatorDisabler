# Testing SCPSL-SpecatorDisabler

The following scenarios are expected to work and shoud be tested when
making changes to the plugin.

Some test require more than one player and are marked with a remark.

## Basic functionality

- Dying by any means puts the player into the tutorial role
- A dead player can respawn as MTF and Chaos

## Options

- When option `tower_window_blockers` is `true` the windows of the tutorial tower are blocked
- When option `tower_workbench` is `true` a workbench and weapons spawn in the tutorial tower

## SCP-049 (Doctor)

- A recently killed player can be revived by SCP-049 and spawns as a zombie
  - Requires 2 players
- A zombie recently killed by another player can be revived one time
  - Requires 3 players (SCP-049, human to be revived, human that kills zombie)

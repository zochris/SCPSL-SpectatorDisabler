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
  - Requires 3 players or 2 players and a dummy (SCP-049, zombie to be revived, human that kills zombie)

## Pocket Dimension
- Items lost in the pocket dimension spawn around players that are not in the tutorial role
  - Requires 3 players (SCP + 2 humanoids), because the last human is killed instantly instead of being
    put into the pocket dimension

## SCP-1576 (Phonograph)
- A player using SCP-1576 can communicate with players in the tutorial role
  - Requires 2 players
- Players in the tutorial role can communicate with each other
  - Requires 2 players
- When starting/stopping SCP-1576 a message is displayed for all players in the tutorial role


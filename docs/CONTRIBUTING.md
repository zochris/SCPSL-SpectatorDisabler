# Contributing to SCPSL-SpecatorDisabler

## Do you have a question?

Open a new discussion on the [discussion page](https://github.com/zochris/SCPSL-SpectatorDisabler/discussions) of this repository.

## Did you find a bug?

- Ensure that the bug is caused by the SpectatorDisabler and not another plugin you are using.
- Ensure that the bug was not already reported by searching the current [issues](https://github.com/zochris/SCPSL-SpectatorDisabler/issues).
- If you can not find an existing issue, [open a new one](https://github.com/zochris/SCPSL-SpectatorDisabler/issues/new). Be sure to include as many details as you can.

## Do you have a suggestion?

- Search the [issues](https://github.com/zochris/SCPSL-SpectatorDisabler/issues) to ensure that your suggestions was not already suggested.
- If you find a matching suggestion consider adding your ideas in a comment or upvote it.
- If you can not find your suggestion, [open a new issue](https://github.com/zochris/SCPSL-SpectatorDisabler/issues/new) and describe your ideas.

## Do you want to contribute code?

Please [open an issue](https://github.com/zochris/SCPSL-SpectatorDisabler/issues/new) or a [discussion](https://github.com/zochris/SCPSL-SpectatorDisabler/discussions) describing your intended changes so others know what you are working on.

### Building

- Checkout the code
- Make your changes
- Run `msbuild` to build (Output will be in `Specator-Disabler/bin/Debug/`)

### Coding conventions

Read the existing code and most things will be apparent. Some things to note:

- Indent with 4 spaces
- Use LF line-endings
- Since this is open-source, please consider the people who will read the code

### Testing

For testing you will need at least two players. Please test the following scenarios:

- Dying by any means puts the player into the tutorial role
- A dead player can respawn as MTF and Chaos
- A recently killed player can be revived by SCP-049 and spawns as a zombie

### Submitting changes

- Fork this repository
- Create a branch in your fork of this repository
- Push changes to your branch
- Submit a Pull-Request against the develop-branch of this repository

## Code of Conduct

Everyone interacting with this repository is expected to follow the [code of conduct](../CODE_OF_CONDUCT.md).

# TODO

## Reach

- [ ] Improve `BrightnessBasedGenerator` implementation.
  - [ ] Switch from ASCII to UTF-8 encoding on all platforms.
- [X] ~~Multi-image~~ / video support.
  - [ ] Genetic algorithm to create GIF from different iterations.
  - [ ] Multi-image tiling
- [ ] Investigate coloring.

## High-level

- [X] Investigate .NET Core 3.0.
  - [X] Investigate C# 8.0.
    - [ ] Investigate nullable reference types
- [ ] Come up with a documentation plan (e.g. publishing to GitHub Pages).
- [ ] How to handle errors in plugins?
- [ ] Implement argument parsing.
  - [ ] Group arguments by prefixes to figure out which ones are passed to plugins. (e.g `--generator-*` and `--renderer-*`).
  - [ ] Allow shortcut for generators and renderers with the same name in the same plugin ( `--package`).
  - [ ] Add interfaces in `Texart.Api` to support common CLI arguments deserialization.

## Chore

- [ ] Convert some of the `Debug.Assert`s to real checks.
  - [ ] Especially in `Texart.Api` since that will be run by public code.
- [ ] Investigate an alternative to `mermaidjs` for diagrams in docs since GitHub doesn't support it.

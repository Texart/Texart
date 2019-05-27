# TODO

## Reach

- [ ] Investigate coloring.

## High-level

- [ ] Investigate .NET Core 3.0.
  - [ ] Investigate C# 8.0 (and nullable reference types).
- [ ] Come up with a documentation plan (e.g. publishing to GitHub Pages).
- [ ] How to handle errors in plugins?
- [ ] Implement argument parsing.
  - [ ] Group arguments by prefixes to figure out which ones are passed to plugins. (e.g `--generator-*` and `--renderer-*`).
  - Add interfaces in `Texart.Api` to support common CLI arguments deserialization.
- [ ] Require `#r "Texart.Api.dll"` or similar directive at the top of `.csx` files. This will improve auto-complete experience.

## Chore

- [ ] Convert some of the `Debug.Assert`s to real checks.
  - [ ] Especially in `Texart.Api` since that will be run by public code.
- [ ] Investigate an alternative to `mermaidjs` for diagrams in docs since GitHub doesn't support it.

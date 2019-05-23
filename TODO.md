# TODO

## High-level

- [ ] Investigate .NET Core 3.0
  - [ ] Investigate C# 8.0 (and nullable reference types)
- [ ] Investigate coloring.
- [ ] Come up with a documentation plan (e.g. publishing to GitHub Pages)
- [ ] How to handle errors in plugins?
- [ ] Implement argument parsing.
  - [ ] Group by prefixes to figure out which arguments are passed to plugins. (e.g `--generator-*` and `--renderer-*`).
- [ ] Investigate JSON for communication with plugins.
  - [X] ~~Investigate whether `Stream` is the right type (or maybe `JObject/JArray`).~~ Decided on `Lazy<JToken>`.

## App-level

- [X] Rename `Texart.Interface` to something like `Texart.Api`.
- [ ] Merge `Texart.ScriptInterface` with `Texart.Interface`.
- [X] Move common helpers (such `ArrayTextBitmap`) to `Texart.Api`.
  - [ ] `Texart.Builtin` and external plugins should both depend on this assembly.

## Chore

- [ ] Convert some of the `Debug.Assert`s to real checks.
  - [ ] Especially in `Texart.Interface` since that will be run by public code.
- [ ] Investigate an alternative to mermaidjs for diagrams in docs since GitHub doesn't support it.

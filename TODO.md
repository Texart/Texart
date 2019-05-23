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
- [X] ~~Investigate using JSON for communication with plugins.~~ Looks good for now.
  - [X] ~~Investigate whether `Stream` is the right type (or maybe `JObject/JArray`).~~ Decided on `Lazy<JToken>`.

## Chore

- [ ] Remove `SkiaPropertyAttribute`
  - [ ] Remove useless wrappers around Skia types, `Color`, `Typeface`, `Bitmap` (or at least make them `static`).
- [X] Rename `Texart.Interface` to something like `Texart.Api`.
- [X] Move common helpers (such `ArrayTextBitmap`) to `Texart.Api`.
  - [ ] `Texart.Builtin` and external plugins should both depend on this assembly.
- [ ] Convert some of the `Debug.Assert`s to real checks.
  - [ ] Especially in `Texart.Api` since that will be run by public code.
- [ ] Investigate an alternative to `mermaidjs` for diagrams in docs since GitHub doesn't support it.

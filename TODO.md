# TODO

## High-level

- [ ] Investigate coloring.
- [ ] Come up with a documentation plan (e.g. publishing to GitHub Pages)
- [ ] How to handle errors in plugins?
- [ ] Implement argument parsing.
  - [ ] Group by prefixes to figure out which arguments are passed to plugins. (e.g `--generator-*` and `--renderer-*`).
- [ ] Investigate JSON for communication with plugins.
  - [ ] Investigate whether `Stream` is the right type (or maybe `JObject/JArray`).

## App-level

- [ ] Rename `Texart.Interface` to something like `Texart.Api`.
- [ ] Merge `Texart.ScriptInterface` with `Texart.Interface`.
- [ ] Move common helpers (such `ArrayTextData`) to `Texart.Api`.
  - [ ] `Texart.Builtin` and external plugins should both depend on this assembly.

## Chore

- [ ] Convert some of the `Debug.Assert`s to real checks.
  - [ ] Especially in `Texart.Interface` since that will be run by public code.
- [ ] Investigate an alternative to mermaidjs for diagrams in docs since GitHub doesn't support it.

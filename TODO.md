# TODO

## High-level

- [ ] Investigate coloring.
- [ ] Come up with a documentation plan (e.g. publishing to GitHub Pages)

## App-level

- [ ] Rename `Texart.Interface` to something like `Texart.Api`.
- [ ] Merge `Texart.ScriptInterface` with `Texart.Interface`.
- [ ] Move common helpers (such `ArrayTextData`) to `Texart.Api`.
  - [ ] `Texart.Builtin` and external plugins should both depend on this assembly.
- [ ] Investigate argument parsing + JSON for communication with plugins.

## Chore

- [ ] Convert some of the `Debug.Assert`s to real checks.
  - [ ] Especially in `Texart.Interface` since that will be run by public code.
- [ ] Investigate an alternative to mermaidjs for diagrams in docs since GitHub doesn't support it.

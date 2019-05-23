# TODO

## High-level

* Investigate coloring.

## App-level

* Rename `Texart.Interface` to something like `Texart.Api`.
* Move common helpers (such `ArrayTextData`) to `Texart.ScriptInterface`.
  * Rename `Texart.ScriptInterface` to something like `Texart.Api.Impl` (but use `Texart.Api` namespace still).
  * `Texart.Builtin` and external plugins shoudl both depend on this assembly.
* Investigate argument parsing + JSON for communication with plugins.

## Chore

* Make `ArrayTextData` part of `Texart.ScriptInterface`.
* Convert some of the `Debug.Assert`s to real checks.
  * Especially in `Texart.Interface` since that will be run by public code.

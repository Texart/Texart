# Documentation

## Assemblies

* `Texart`
  * The executable assembly - contains `Main` in `Program.cs`.
* `Texart.Api`
  * The commonly used domain interfaces and types, including `ITxTextBitmap`, `Font` etc.
  * Also contains reference implementations for some interfaces (e.g. `TxArrayTextBitmap`).
  * This assembly is accessible from plugins. It's important to not depend on other `Texart.*` assemblies. The job of this assembly is to define the vocabulary types.
* `Texart.Builtin`
  * Bundled default implementations for `ITxTextBitmapGenerator`s and `ITxTextBitmapRenderer`s.
  * A "plugin" that is bundled with Texart.
* `Texart.Plugins`
  * Types and logic that faciliates loading "plugins" that provide implementations for `ITxTextBitmapGenerator` and `ITxTextBitmapRenderer`.
  * Allows loading assemblies as well as scripts.
* `Texart.ScriptInterface` (to be renamed)
  * The (additional) API that is provided to plugins. Note that plugins also have access to `Texart.Api`.

## Important Types

* [`ITxTextBitmap`](i-tx-text-types.md#itxtextbitmap)
* [`ITxTextBitmapGenerator`](i-tx-text-types.md#itxtextbitmapgenerator)
* [`ITxTextBitmapRenderer`](i-tx-text-types.md#itxtextbitmaprenderer)

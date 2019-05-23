# Documentation

## Assemblies

* `Texart`
  * The executable assembly - contains `Main` in `Program.cs`.
* `Texart.Api`
  * The commonly used domain interfaces and types, including `ITextBitmap`, `Font` etc.
  * Also contains reference implementations for some interfaces (e.g. `ArrayTextBitmap`).
  * This assembly is accessible from plugins. It's important to not depend on other `Texart.*` assemblies. The job of this assembly is to define the vocabulary types.
* `Texart.Builtin`
  * Bundled default implementations for `ITextBitmapGenerator`s and `ITextBitmapRenderer`s.
  * A "plugin" that is bundled with Texart.
* `Texart.Plugins`
  * Types and logic that faciliates loading "plugins" that provide implementations for `ITextBitmapGenerator` and `ITextBitmapRenderer`.
  * Allows loading assemblies as well as scripts.
* `Texart.ScriptInterface` (to be renamed)
  * The (additional) API that is provided to plugins. Note that plugins also have access to `Texart.Api`.

## Important Types

* [`ITextBitmap`](i-text-types.md#itextbitmap)
* [`ITextBitmapGenerator`](i-text-types.md#itextbitmapgenerator)
* [`ITextBitmapRenderer`](i-text-types.md#itextbitmaprenderer)

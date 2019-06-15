# Documentation

Welcome to the Texart API documentation. If you're looking for a high-level overview of Texart's architecture and/or plugin development (including examples), you're in the right place!

Auto-generated Texart API _reference_ documentation is available [here](https://texart.github.io/Texart/).

## How To Read The Docs

Texart documentation is completely browseable within GitHub! The [table of contents](#table-of-contents) is a good place to start browsing. Most of the documentation is grouped by topics.

Before you proceed, you should perform the pre-requisite steps:

* Install [GitHub + Mermaid](https://github.com/BackMarket/github-mermaid-extension) browser extension.
  * Diagrams in the docs are written with [mermaid](https://mermaidjs.github.io/). GitHub-flavored markdown does not support mermaid diagrams. GitLab *does* support mermaid natively. You may browse the [GitLab mirror](https://gitlab.com/Texart/Texart) (note that browsing the documentation on GitLab mirror is not officially supported, but it *should* mostly work anyways).

## Table of Contents

TODO

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

## Important Types

* [`ITxTextBitmap`](i-tx-text-types.md#itxtextbitmap)
* [`ITxTextBitmapGenerator`](i-tx-text-types.md#itxtextbitmapgenerator)
* [`ITxTextBitmapRenderer`](i-tx-text-types.md#itxtextbitmaprenderer)

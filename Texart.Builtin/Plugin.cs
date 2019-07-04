using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Texart.Builtin.Generators;
using Texart.Builtin.Renderers;
using Texart.Api;

namespace Texart.Builtin
{
    /// <summary>
    /// The plugin implementation for the built in <see cref="ITxTextBitmapGenerator"/> and <see cref="ITxTextBitmapRenderer"/> types.
    /// </summary>
    public sealed class Plugin : TxPluginBuilder.Base
    {
        private static TxPluginBuilder BuilderDescription => new TxPluginBuilder()
            .AddGenerator(typeof(BrightnessBasedBitmapGenerator), BrightnessBasedBitmapGenerator.Create)
            .AddDefaultGenerator(BrightnessBasedBitmapGenerator.Create)
            .AddRenderer(typeof(FontBitmapRenderer), FontBitmapRenderer.Create)
            .AddRenderer(typeof(StringBitmapRenderer), StringBitmapRenderer.Create)
            .AddDefaultRenderer(FontBitmapRenderer.Create);

        public Plugin() : base(BuilderDescription) { }
    }
}
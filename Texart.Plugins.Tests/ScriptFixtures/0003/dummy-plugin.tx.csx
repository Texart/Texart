#r "SkiaSharp.dll"
#r "Newtonsoft.Json.dll"
#r "Texart.Api.dll"

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SkiaSharp;
using Texart.Api;

sealed class DummyPlugin : IPlugin
{
    public IEnumerable<string> AvailableGenerators => ImmutableArray<string>.Empty;

    public TxFactory<ITextBitmapGenerator, TxArguments> LookupGenerator(string name)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> AvailableRenderers => ImmutableArray<string>.Empty;

    public TxFactory<ITextBitmapRenderer, TxArguments> LookupRenderer(string name)
    {
        throw new NotImplementedException();
    }
}

return new DummyPlugin();

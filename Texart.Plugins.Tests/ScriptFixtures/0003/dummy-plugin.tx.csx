#r "SkiaSharp.dll"
#r "Texart.Api.dll"

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SkiaSharp;
using Texart.Api;

sealed class DummyPlugin : ITxPlugin
{
    public IEnumerable<string> AvailableGenerators => ImmutableArray<string>.Empty;

    public TxFactory<ITxTextBitmapGenerator, TxArguments> LookupGenerator(string name)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> AvailableRenderers => ImmutableArray<string>.Empty;

    public TxFactory<ITxTextBitmapRenderer, TxArguments> LookupRenderer(string name)
    {
        throw new NotImplementedException();
    }
}

return new DummyPlugin();

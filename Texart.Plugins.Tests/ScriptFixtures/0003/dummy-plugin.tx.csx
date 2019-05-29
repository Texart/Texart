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

    public TxPluginResource<ITxTextBitmapGenerator> LookupGenerator(string name)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> AvailableRenderers => ImmutableArray<string>.Empty;

    public TxPluginResource<ITxTextBitmapRenderer> LookupRenderer(string name)
    {
        throw new NotImplementedException();
    }
}

return new DummyPlugin();

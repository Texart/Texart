#r "Texart.Api.dll"

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using Texart.Api;

sealed class DummyPlugin : IPlugin
{
    public IEnumerable<string> AvailableGenerators => ImmutableArray<string>.Empty;

    public TxFactory<ITextBitmapGenerator, Lazy<JToken>> LookupGenerator(string name)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> AvailableRenderers => ImmutableArray<string>.Empty;

    public TxFactory<ITextBitmapRenderer, Lazy<JToken>> LookupRenderer(string name)
    {
        throw new NotImplementedException();
    }
}

return new DummyPlugin();

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using Texart.Api;

sealed class DummyPlugin : IPlugin
{
    public IEnumerable<string> AvailableGenerators => ImmutableArray<string>.Empty;

    public Factory<ITextBitmapGenerator, Lazy<JToken>> LookupGenerator(string name)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> AvailableRenderers => ImmutableArray<string>.Empty;

    public Factory<ITextBitmapRenderer, Lazy<JToken>> LookupRenderer(string name)
    {
        throw new NotImplementedException();
    }
}

return new DummyPlugin();

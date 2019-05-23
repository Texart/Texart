using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Texart.Api
{
    public interface IPlugin
    {
        IEnumerable<string> AvailableGenerators { get; }
        Factory<ITextBitmapGenerator, Lazy<JToken>> LookupGenerator(string name);

        IEnumerable<string> AvailableRenderers { get; }
        Factory<ITextBitmapRenderer, Lazy<JToken>> LookupRenderer(string name);
    }
}
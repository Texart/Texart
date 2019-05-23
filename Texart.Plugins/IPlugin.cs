using System.Collections.Generic;
using Texart.Api;

namespace Texart.Plugins
{
    public interface IPlugin
    {
        IEnumerable<string> AvailableGenerators { get; }
        Factory<ITextBitmapGenerator> LookupGenerator(string name);

        IEnumerable<string> AvailableRenderers { get; }
        Factory<ITextBitmapRenderer> LookupRenderer(string name);
    }
}
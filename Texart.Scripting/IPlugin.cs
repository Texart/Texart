using System.Collections.Generic;
using Texart.Interface;

namespace Texart.Plugins
{
    public interface IPlugin
    {
        IEnumerable<string> AvailableGenerators { get; }
        Factory<ITextGenerator> LookupGenerator(string name);

        IEnumerable<string> AvailableRenderers { get; }
        Factory<ITextRenderer> LookupRenderer(string name);
    }
}
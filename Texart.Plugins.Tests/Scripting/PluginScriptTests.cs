using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Texart.Plugins.Scripting;

namespace Texart.Plugins.Tests.Scripting
{
    internal class PluginScriptTests
    {
        [Test]
        public async Task AllowsCompilation()
        {
            var script = PluginScript.From(SourceFile.Load("../../../scripts/hello.csx"));
            var result = (await script.RunAsync()).ReturnValue;
        }
    }
}

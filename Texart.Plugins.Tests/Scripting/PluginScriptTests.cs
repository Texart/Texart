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
            const int fixture = 1;
            // TODO: set up actual tests
            var script = PluginScript.LoadFrom(ScriptFixtures.GetPath(fixture, "hello.csx"));
            var result = (await script.RunAsync()).ReturnValue;
        }
    }
}

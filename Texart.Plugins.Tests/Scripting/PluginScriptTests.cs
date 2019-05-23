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
            var script = ScriptFixtures.LoadFrom(fixture, "hello.csx");
            var result = (await script.RunAsync()).ReturnValue;
        }

        [Test]
        public async Task AllowsLoadingAssembly()
        {
            const int fixture = 2;
            var script = ScriptFixtures.LoadFrom<int>(fixture, "load-assembly.csx");
            var result = (await script.RunAsync()).ReturnValue;
            Assert.AreEqual(42, result);
        }

        [Test]
        public async Task AllowsLoadingAssemblyWithScheme()
        {
            const int fixture = 2;
            var script = ScriptFixtures.LoadFrom<int>(fixture, "load-assembly-with-scheme.csx");
            var result = (await script.RunAsync()).ReturnValue;
            Assert.AreEqual(42, result);
        }
    }
}

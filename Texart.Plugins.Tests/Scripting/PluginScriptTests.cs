using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Texart.Plugins.Tests.Scripting
{
    internal class PluginScriptTests
    {
        [Test]
        public async Task AllowsLoadingScript()
        {
            const int fixture = 1;
            var script = await ScriptFixtures.LoadFrom<int>(fixture, "load-script.tx.csx");
            var result = (await script.RunAsync()).ReturnValue;
            Assert.AreEqual(42, result);
        }

        [Test]
        public async Task AllowsLoadingScriptWithScheme()
        {
            const int fixture = 1;
            // TODO: set up actual tests
            var script = await ScriptFixtures.LoadFrom<int>(fixture, "load-script-with-scheme.tx.csx");
            var result = (await script.RunAsync()).ReturnValue;
            Assert.AreEqual(42, result);
        }

        [Test]
        public async Task AllowsLoadingAssembly()
        {
            const int fixture = 2;
            var script = await ScriptFixtures.LoadFrom<int>(fixture, "load-assembly.tx.csx");
            var result = (await script.RunAsync()).ReturnValue;
            Assert.AreEqual(42, result);
        }

        [Test]
        public async Task AllowsLoadingAssemblyWithScheme()
        {
            const int fixture = 2;
            var script = await ScriptFixtures.LoadFrom<int>(fixture, "load-assembly-with-scheme.tx.csx");
            var result = (await script.RunAsync()).ReturnValue;
            Assert.AreEqual(42, result);
        }

        [Test]
        public async Task AllowsDummyPlugin()
        {
            const int fixture = 3;
            var script = await ScriptFixtures.LoadFrom(fixture, "dummy-plugin.tx.csx");
            var result = (await script.RunAsync()).ReturnValue;
            Assert.AreEqual(0, result.AvailableGenerators.Count());
            Assert.AreEqual(0, result.AvailableRenderers.Count());
            Assert.Throws<NotImplementedException>(() => result.LookupGenerator(null));
            Assert.Throws<NotImplementedException>(() => result.LookupRenderer(null));
            Assert.AreEqual("DummyPlugin", result.GetType().Name);
        }
    }
}

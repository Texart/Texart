using System;
using System.Collections.Generic;
using NUnit.Framework;
using Texart.Api;

namespace Texart.Plugins.Tests
{
    using Locator = TxPluginResourceLocator;
    using RelativeLocator = TxPluginResourceLocator.RelativeLocator;
    using BadPluginAssemblyException = PrebuiltPlugin.BadPluginAssemblyException;

    [TestFixture]
    internal class PrebuiltPluginTests
    {
        [Test]
        public void AllowsSimplePlugin()
        {
            var plugin = FromTypes(typeof(Plugin1), typeof(Plugin0));
            Assert.AreEqual(plugin.GetType(), typeof(Plugin1));
        }

        [Test]
        public void RejectsWhenNoPlugins()
        {
            var ex = Assert.Throws<BadPluginAssemblyException>(() => FromTypes(typeof(Plugin0)));
            Assert.IsTrue(ex.Message.Contains("No types found"));
        }

        [Test]
        public void RejectsWhenMultiplePlugins()
        {
            var ex = Assert.Throws<BadPluginAssemblyException>(
                () => FromTypes(typeof(Plugin1), typeof(Plugin2)));
            Assert.IsTrue(ex.Message.Contains("Multiple types found"));
        }

        [Test]
        public void RejectsPluginWithoutNoArgsCtor()
        {
            var ex = Assert.Throws<BadPluginAssemblyException>(
                () => FromTypes(typeof(PluginWithoutDefaultCtor), typeof(Plugin0)));
            Assert.IsTrue(ex.Message.Contains("does not have a public, no-args constructor"));
        }

        [Test]
        public void RejectsPluginWithPrivateNoArgsCtor()
        {
            var ex = Assert.Throws<BadPluginAssemblyException>(
                () => FromTypes(typeof(PluginWithPrivateDefaultCtor), typeof(Plugin0)));
            Assert.IsTrue(ex.Message.Contains("does not have a public, no-args constructor"));
        }

        [Test]
        public void RejectsPluginThatThrowsInCtor()
        {
            var ex = Assert.Throws<BadPluginAssemblyException>(
                () => FromTypes(typeof(PluginThatThrowsInCtor), typeof(Plugin0)));
            Assert.IsTrue(ex.Message.Contains("constructor threw an exception"));
        }

        [Test]
        public void RejectsPluginThatIsAbstract()
        {
            var ex = Assert.Throws<BadPluginAssemblyException>(
                () => FromTypes(typeof(PluginThatIsAbstract), typeof(Plugin0)));
            Assert.IsTrue(ex.Message.Contains("the plugin is abstract"));
        }

        private static ITxPlugin FromTypes(params Type[] types) => PrebuiltPlugin.GetPluginFromTypes(types);

        private abstract class DummyPluginBase : ITxPlugin
        {
            public IEnumerable<RelativeLocator> AvailableGenerators =>
                throw new NotImplementedException();
            public TxPluginResource<ITxTextBitmapGenerator> LookupGenerator(Locator locator) =>
                throw new NotImplementedException();
            public IEnumerable<RelativeLocator> AvailableRenderers =>
                throw new NotImplementedException();
            public TxPluginResource<ITxTextBitmapRenderer> LookupRenderer(Locator locator) =>
                throw new NotImplementedException();
            public IEnumerable<RelativeLocator> AvailablePackages =>
                throw new NotImplementedException();
            public (RelativeLocator generator, RelativeLocator renderer) LookupPackage(Locator locator) =>
                throw new NotImplementedException();
            public void PrintHelp(ITxConsole console) =>
                throw new NotImplementedException();
            public void PrintHelp(ITxConsole console, TxPluginResourceKind resourceKind, Locator locator) =>
                throw new NotImplementedException();
        }

        private class Plugin0 : DummyPluginBase { }

        [TxPlugin]
        private class Plugin1 : DummyPluginBase { }

        [TxPlugin]
        private class Plugin2 : DummyPluginBase { }

        [TxPlugin]
        private class PluginWithoutDefaultCtor : DummyPluginBase
        {
            // ReSharper disable once UnusedParameter.Local
            public PluginWithoutDefaultCtor(int dummy) { }
        }

        [TxPlugin]
        private class PluginWithPrivateDefaultCtor : DummyPluginBase
        {
            private PluginWithPrivateDefaultCtor() { }
        }

        [TxPlugin]
        private class PluginThatThrowsInCtor : DummyPluginBase
        {
            public PluginThatThrowsInCtor() => throw new Exception();
        }

        [TxPlugin]
        private abstract class PluginThatIsAbstract : DummyPluginBase
        {
            // ReSharper disable once PublicConstructorInAbstractClass
            // ReSharper disable once EmptyConstructor
            public PluginThatIsAbstract() { }
        }
    }
}
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Texart.Api.Tests
{
    using Locator = TxPluginResourceLocator;
    using RelativeLocator = TxPluginResourceLocator.RelativeLocator;

    [TestFixture]
    internal class TxPluginTests
    {
        private class AllowsRelativeRedirectPlugin : PluginBase
        {
            public override TxPluginResource<ITxTextBitmapGenerator> LookupGenerator(Locator locator) =>
                TxPluginResource.RedirectGenerator(locator
                    .WithScheme("other-scheme")
                    .WithAssemblyPath("/other/plugin.dll")
                    .WithRelativeResource("/other/resource"), args => args);
            public override TxPluginResource<ITxTextBitmapRenderer> LookupRenderer(Locator locator) =>
                TxPluginResource.RedirectRenderer(locator
                    .WithScheme("other-scheme")
                    .WithAssemblyPath("/other/plugin.dll")
                    .WithRelativeResource("/other/resource"), args => args);
        }

        [Test]
        public void AllowsRelativeRedirect()
        {
            ITxPlugin plugin = new AllowsRelativeRedirectPlugin();
            var sourceLocator = Locator.Of("tx:///plugin.dll:resource");
            var expectedRedirectLocation = Locator.Of("other-scheme:////other/plugin.dll:/other/resource");
            var expectedArgs = new TxArguments(new Dictionary<string, string> { { "hello", "world" } });

            var generatorRedirect = plugin.LookupGenerator(sourceLocator).Redirect;
            var rendererRedirect = plugin.LookupRenderer(sourceLocator).Redirect;

            Assert.AreEqual(expectedRedirectLocation, generatorRedirect.Locator);
            Assert.AreEqual(expectedArgs, generatorRedirect.ArgumentsTransformer(expectedArgs));

            Assert.AreEqual(expectedRedirectLocation, rendererRedirect.Locator);
            Assert.AreEqual(expectedArgs, rendererRedirect.ArgumentsTransformer(expectedArgs));
        }

        /// <summary>
        /// Base plugin for implementing plugin tests.
        /// </summary>
        private class PluginBase : ITxPlugin
        {
            public IEnumerable<RelativeLocator> AvailableGenerators =>
                throw new NotImplementedException();
            public virtual TxPluginResource<ITxTextBitmapGenerator> LookupGenerator(Locator locator) =>
                throw new NotImplementedException();
            public virtual IEnumerable<RelativeLocator> AvailableRenderers =>
                throw new NotImplementedException();
            public virtual TxPluginResource<ITxTextBitmapRenderer> LookupRenderer(Locator locator) =>
                throw new NotImplementedException();
            public virtual IEnumerable<RelativeLocator> AvailablePackages =>
                throw new NotImplementedException();
            public virtual (RelativeLocator generator, RelativeLocator renderer) LookupPackage(Locator locator) =>
                throw new NotImplementedException();
            public virtual void PrintHelp(ITxConsole console) =>
                throw new NotImplementedException();
            public virtual void PrintHelp(ITxConsole console, TxPluginResourceKind resourceKind, Locator locator) =>
                throw new NotImplementedException();
        }
    }
}
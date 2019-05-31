using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Texart.Api.Tests
{
    [TestFixture]
    internal class TxPluginTests
    {
        private class AllowsRelativeRedirectPlugin : PluginBase
        {
            public override TxPluginResource<ITxTextBitmapGenerator> LookupGenerator(TxPluginResourceLocator locator) =>
                TxPluginResource.OfGeneratorLocator(locator
                    .WithScheme("other-scheme")
                    .WithAssemblyPath("/other/plugin.dll")
                    .WithRelativeResource("/other/resource"));
            public override TxPluginResource<ITxTextBitmapRenderer> LookupRenderer(TxPluginResourceLocator locator) =>
                TxPluginResource.OfRendererLocator(locator
                    .WithScheme("other-scheme")
                    .WithAssemblyPath("/other/plugin.dll")
                    .WithRelativeResource("/other/resource"));
        }

        [Test]
        public void AllowsRelativeRedirect()
        {
            ITxPlugin plugin = new AllowsRelativeRedirectPlugin();
            var sourceLocator = TxPluginResourceLocator.Of("tx:///plugin.dll:resource");
            var expectedRedirect = TxPluginResourceLocator.Of("other-scheme:////other/plugin.dll:/other/resource");
            Assert.AreEqual(expectedRedirect, plugin.LookupGenerator(sourceLocator).Locator);
            Assert.AreEqual(expectedRedirect, plugin.LookupRenderer(sourceLocator).Locator);
        }

        /// <summary>
        /// Base plugin for implementing plugin tests.
        /// </summary>
        private class PluginBase : ITxPlugin
        {
            public IEnumerable<TxPluginResourceLocator.RelativeLocator> AvailableGenerators =>
                throw new NotImplementedException();
            public virtual TxPluginResource<ITxTextBitmapGenerator> LookupGenerator(TxPluginResourceLocator locator) =>
                throw new NotImplementedException();
            public virtual IEnumerable<TxPluginResourceLocator.RelativeLocator> AvailableRenderers =>
                throw new NotImplementedException();
            public virtual TxPluginResource<ITxTextBitmapRenderer> LookupRenderer(TxPluginResourceLocator locator) =>
                throw new System.NotImplementedException();
        }
    }
}
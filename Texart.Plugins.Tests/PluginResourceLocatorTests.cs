using System;
using NUnit.Framework;

namespace Texart.Plugins.Tests
{
    internal class PluginResourceLocatorTests
    {
        [Test]
        public void AllowsAssemblyAndResource()
        {
            var locator = PluginResourceLocator.FromUri("file:///plugins/Texart.SomePlugin.dll:SomePath/SomeResource");
            Assert.AreEqual(new ReferenceScheme("file"), locator.Scheme);
            Assert.AreEqual("plugins/Texart.SomePlugin.dll", locator.AssemblyPath);
            Assert.AreEqual(new [] { "plugins", "Texart.SomePlugin.dll" }, locator.AssemblySegments);
            Assert.AreEqual("SomePath/SomeResource", locator.ResourcePath);
            Assert.AreEqual(new[] { "SomePath", "SomeResource" }, locator.ResourceSegments);
        }

        [Test]
        public void AllowsEmptyResource()
        {
            var locator = PluginResourceLocator.FromUri("tx:///plugin.dll:");
            Assert.AreEqual(new ReferenceScheme("tx"), locator.Scheme);
            Assert.AreEqual("plugin.dll", locator.AssemblyPath);
            Assert.AreEqual("", locator.ResourcePath);
            Assert.AreEqual(new[] { "" }, locator.ResourceSegments);
        }

        [Test]
        public void AllowsColonsInAssemblyPath()
        {
            var locator = PluginResourceLocator.FromUri("tx:///path:to/plugin:foo.dll:resource/path");
            Assert.AreEqual(new ReferenceScheme("tx"), locator.Scheme);
            Assert.AreEqual("path:to/plugin:foo.dll", locator.AssemblyPath);
            Assert.AreEqual(new [] { "path:to", "plugin:foo.dll" }, locator.AssemblySegments);
            Assert.AreEqual("resource/path", locator.ResourcePath);
            Assert.AreEqual(new[] { "resource", "path" }, locator.ResourceSegments);
        }

        // Also tests against relative URLs
        [Test]
        public void RejectsNoScheme() => AssertInvalidLocator("///a");

        [Test]
        public void RejectsNoColon() => AssertInvalidLocator("tx:///a");

        [Test]
        public void RejectsAuthority() => AssertInvalidLocator("tx://authority/a:c");

        [Test]
        public void RejectsNoPath() => AssertInvalidLocator("tx:///");

        [Test]
        public void RejectsQuery() => AssertInvalidLocator("tx:///a:c?hello=world");

        [Test]
        public void RejectsEmptyQuery() => AssertInvalidLocator("tx:///a:c?");

        [Test]
        public void RejectsFragment() => AssertInvalidLocator("tx:///a:c#hello");

        [Test]
        public void RejectsEmptyFragment() => AssertInvalidLocator("tx:///a:c#");

        private static void AssertInvalidLocator(string uri)
        {
            var ex = Assert.Throws<ArgumentException>(() => PluginResourceLocator.FromUri(new Uri(uri)));
            Assert.IsTrue(
                ex.Message.StartsWith("URI "),
                "Exception was thrown but not because of invalid locator");
        }
    }
}
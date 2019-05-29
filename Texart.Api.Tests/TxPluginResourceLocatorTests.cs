using System;
using NUnit.Framework;

namespace Texart.Api.Tests
{
    internal class TxPluginResourceLocatorTests
    {
        [Test]
        public void AllowsAbsoluteUriAssemblyAndResource()
        {
            var locator = TxPluginResourceLocator.FromUri("file:///plugins/Texart.SomePlugin.dll:SomePath/SomeResource");
            Assert.AreEqual(new TxReferenceScheme("file"), locator.Scheme);
            Assert.AreEqual("plugins/Texart.SomePlugin.dll", locator.AssemblyPath);
            Assert.AreEqual(new [] { "plugins", "Texart.SomePlugin.dll" }, locator.AssemblySegments);
            Assert.AreEqual("SomePath/SomeResource", locator.ResourcePath);
            Assert.AreEqual(new[] { "SomePath", "SomeResource" }, locator.ResourceSegments);

            Assert.IsTrue(TxPluginResourceLocator.IsWellFormedResourceLocatorUri(
                new Uri("file:///plugins/Texart.SomePlugin.dll:SomePath/SomeResource")));
        }

        [Test]
        public void AllowsEmptyResource()
        {
            var locator = TxPluginResourceLocator.FromUri("tx:///plugin.dll:");
            Assert.AreEqual(new TxReferenceScheme("tx"), locator.Scheme);
            Assert.AreEqual("plugin.dll", locator.AssemblyPath);
            Assert.AreEqual("", locator.ResourcePath);
            Assert.AreEqual(new[] { "" }, locator.ResourceSegments);
        }

        [Test]
        public void AllowsColonsInAssemblyPath()
        {
            var locator = TxPluginResourceLocator.FromUri("tx:///path:to/plugin:foo.dll:resource/path");
            Assert.AreEqual(new TxReferenceScheme("tx"), locator.Scheme);
            Assert.AreEqual("path:to/plugin:foo.dll", locator.AssemblyPath);
            Assert.AreEqual(new [] { "path:to", "plugin:foo.dll" }, locator.AssemblySegments);
            Assert.AreEqual("resource/path", locator.ResourcePath);
            Assert.AreEqual(new[] { "resource", "path" }, locator.ResourceSegments);
        }

        [Test]
        public void AllowsEmptySegments()
        {
            var locator = TxPluginResourceLocator.FromUri("tx:///:");
            Assert.AreEqual(new TxReferenceScheme("tx"), locator.Scheme);
            Assert.AreEqual(string.Empty, locator.AssemblyPath);
            Assert.AreEqual(new [] { string.Empty }, locator.AssemblySegments);
            Assert.AreEqual(string.Empty, locator.ResourcePath);
            Assert.AreEqual(new[] { string.Empty }, locator.ResourceSegments);
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
            var ex = Assert.Throws<ArgumentException>(() => TxPluginResourceLocator.FromUri(new Uri(uri)));
            Assert.IsTrue(
                ex.Message.StartsWith("URI "),
                "Exception was thrown but not because of invalid locator");
            Assert.IsFalse(TxPluginResourceLocator.IsWellFormedResourceLocatorUri(new Uri(uri)));
        }

        /// <summary>
        /// Tests for <see cref="TxPluginResourceLocator.Relative"/>.
        /// </summary>
        internal class RelativeTests
        {
            [Test]
            public void AllowsRelativeAssemblyAndResource()
            {
                var relative = TxPluginResourceLocator.Relative.FromRelativePath("/plugins/Texart.SomePlugin.dll:SomePath/SomeResource");
                Assert.AreEqual("plugins/Texart.SomePlugin.dll", relative.AssemblyPath);
                Assert.AreEqual(new [] { "plugins", "Texart.SomePlugin.dll" }, relative.AssemblySegments);
                Assert.AreEqual("SomePath/SomeResource", relative.ResourcePath);
                Assert.AreEqual(new[] { "SomePath", "SomeResource" }, relative.ResourceSegments);

                Assert.IsTrue(TxPluginResourceLocator.Relative.IsWellFormedRelativePathString(
                    "/plugins/Texart.SomePlugin.dll:SomePath/SomeResource"));
            }

            [Test]
            public void AllowsEmptyResource()
            {
                var relative = TxPluginResourceLocator.Relative.FromRelativePath("/plugin.dll:");
                Assert.AreEqual("plugin.dll", relative.AssemblyPath);
                Assert.AreEqual("", relative.ResourcePath);
                Assert.AreEqual(new[] { "" }, relative.ResourceSegments);
            }

            [Test]
            public void AllowsColonsInAssemblyPath()
            {
                var relative = TxPluginResourceLocator.Relative.FromRelativePath("/path:to/plugin:foo.dll:resource/path");
                Assert.AreEqual("path:to/plugin:foo.dll", relative.AssemblyPath);
                Assert.AreEqual(new [] { "path:to", "plugin:foo.dll" }, relative.AssemblySegments);
                Assert.AreEqual("resource/path", relative.ResourcePath);
                Assert.AreEqual(new[] { "resource", "path" }, relative.ResourceSegments);
            }

            [Test]
            public void AllowsEmptySegments()
            {
                var relative = TxPluginResourceLocator.Relative.FromRelativePath("/:");
                Assert.AreEqual(string.Empty, relative.AssemblyPath);
                Assert.AreEqual(new [] { string.Empty }, relative.AssemblySegments);
                Assert.AreEqual(string.Empty, relative.ResourcePath);
                Assert.AreEqual(new[] { string.Empty }, relative.ResourceSegments);
            }

            [Test]
            public void RejectsNoPath() => AssertInvalidRelative("/");

            [Test]
            public void RejectsQuery() => AssertInvalidRelative("/a:c?hello=world");

            [Test]
            public void RejectsEmptyQuery() => AssertInvalidRelative("/a:c?");

            [Test]
            public void RejectsFragment() => AssertInvalidRelative("/a:c#hello");

            [Test]
            public void RejectsEmptyFragment() => AssertInvalidRelative("/a:c#");

            private static void AssertInvalidRelative(string relativePath)
            {
                var ex = Assert.Throws<ArgumentException>(
                    () => TxPluginResourceLocator.Relative.FromRelativePath(relativePath));
                Assert.IsTrue(
                    ex.Message.StartsWith("URI "),
                    "Exception was thrown but not because of invalid relative");
                Assert.IsFalse(
                    TxPluginResourceLocator.Relative.IsWellFormedRelativePathString(relativePath));
            }
        }
    }
}
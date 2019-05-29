using System;
using NUnit.Framework;

namespace Texart.Api.Tests
{
    internal class TxPluginResourceLocatorTests
    {
        [Test]
        public void AllowsAbsoluteUriAssemblyAndResource()
        {
            var locator = TxPluginResourceLocator.Of("file:///plugins/Texart.SomePlugin.dll:SomePath/SomeResource");
            Assert.AreEqual(new TxReferenceScheme("file"), locator.Scheme);
            Assert.AreEqual("plugins/Texart.SomePlugin.dll", locator.AssemblyPath);
            Assert.AreEqual(new [] { "plugins", "Texart.SomePlugin.dll" }, locator.AssemblySegments);
            Assert.AreEqual(TxPluginResourceLocator.OfRelativeResource("SomePath/SomeResource"), locator.RelativeResource);
            Assert.AreEqual("SomePath/SomeResource", locator.ResourcePath);
            Assert.AreEqual(new[] { "SomePath", "SomeResource" }, locator.ResourceSegments);

            Assert.IsTrue(TxPluginResourceLocator.IsWellFormedResourceLocatorUri(
                new Uri("file:///plugins/Texart.SomePlugin.dll:SomePath/SomeResource")));
        }

        [Test]
        public void AllowsEmptyResource()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugin.dll:");
            Assert.AreEqual(new TxReferenceScheme("tx"), locator.Scheme);
            Assert.AreEqual("plugin.dll", locator.AssemblyPath);
            Assert.AreEqual(TxPluginResourceLocator.OfRelativeResource(string.Empty), locator.RelativeResource);
            Assert.AreEqual(string.Empty, locator.ResourcePath);
            Assert.AreEqual(new[] { string.Empty }, locator.ResourceSegments);
        }

        [Test]
        public void AllowsColonsInAssemblyPath()
        {
            var locator = TxPluginResourceLocator.Of("tx:///path:to/plugin:foo.dll:resource/path");
            Assert.AreEqual(new TxReferenceScheme("tx"), locator.Scheme);
            Assert.AreEqual("path:to/plugin:foo.dll", locator.AssemblyPath);
            Assert.AreEqual(new [] { "path:to", "plugin:foo.dll" }, locator.AssemblySegments);
            Assert.AreEqual(TxPluginResourceLocator.OfRelativeResource("resource/path"), locator.RelativeResource);
            Assert.AreEqual("resource/path", locator.ResourcePath);
            Assert.AreEqual(new[] { "resource", "path" }, locator.ResourceSegments);
        }

        [Test]
        public void AllowsEmptyAssemblyAndResource()
        {
            var locator = TxPluginResourceLocator.Of("tx:///:");
            Assert.AreEqual(new TxReferenceScheme("tx"), locator.Scheme);
            Assert.AreEqual(string.Empty, locator.AssemblyPath);
            Assert.AreEqual(new [] { string.Empty }, locator.AssemblySegments);
            Assert.AreEqual(TxPluginResourceLocator.OfRelativeResource(string.Empty), locator.RelativeResource);
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
            var ex = Assert.Throws<ArgumentException>(() => TxPluginResourceLocator.Of(uri));
            Assert.IsTrue(
                ex.Message.StartsWith("URI "),
                "Exception was thrown but not because of invalid locator");
            Assert.IsFalse(TxPluginResourceLocator.IsWellFormedResourceLocatorUri(new Uri(uri)));
        }

        /// <summary>
        /// Tests for <see cref="TxPluginResourceLocator.RelativeResourceLocator"/>.
        /// </summary>
        internal class RelativeResourceTests
        {
            [Test]
            public void AllowsResource()
            {
                var relative = TxPluginResourceLocator.OfRelativeResource("SomePath/SomeResource");
                Assert.AreEqual("SomePath/SomeResource", relative.ResourcePath);
                Assert.AreEqual(new[] { "SomePath", "SomeResource" }, relative.ResourceSegments);
                Assert.AreEqual(
                    TxPluginResourceLocator.Of("tx:///assembly.dll:SomePath/SomeResource").RelativeResource,
                    relative);

                Assert.IsTrue(TxPluginResourceLocator.IsWellFormedRelativeResourceString(
                    "SomePath/SomeResource"));
            }

            [Test]
            public void AllowsEmptyResource()
            {
                var relative = TxPluginResourceLocator.OfRelativeResource(string.Empty);
                Assert.AreEqual(string.Empty, relative.ResourcePath);
                Assert.AreEqual(new[] { string.Empty }, relative.ResourceSegments);
                Assert.AreEqual(
                    TxPluginResourceLocator.Of("tx:///assembly.dll:").RelativeResource,
                    relative);
            }

            [Test]
            public void AllowsLeadingSlash()
            {
                var relative = TxPluginResourceLocator.OfRelativeResource("/resource/path");
                Assert.AreEqual("/resource/path", relative.ResourcePath);
                Assert.AreEqual(new[] { string.Empty, "resource", "path" }, relative.ResourceSegments);
                Assert.AreEqual(
                    TxPluginResourceLocator.Of("tx:///assembly.dll:/resource/path").RelativeResource,
                    relative);
            }

            [Test]
            public void AllowsEmptyWithLeadingSlash()
            {
                var relative = TxPluginResourceLocator.OfRelativeResource("/");
                Assert.AreEqual("/", relative.ResourcePath);
                Assert.AreEqual(new[] { string.Empty, string.Empty }, relative.ResourceSegments);
            }

            [Test]
            public void RejectsQuery() => AssertInvalidRelative("c?hello=world");

            [Test]
            public void RejectsEmptyQuery() => AssertInvalidRelative("c?");

            [Test]
            public void RejectsFragment() => AssertInvalidRelative("c#hello");

            [Test]
            public void RejectsEmptyFragment() => AssertInvalidRelative("c#");

            private static void AssertInvalidRelative(string relativePath)
            {
                var ex = Assert.Throws<ArgumentException>(
                    () => TxPluginResourceLocator.OfRelativeResource(relativePath));
                Assert.IsTrue(
                    ex.Message.StartsWith("URI "),
                    "Exception was thrown but not because of invalid relative");
                Assert.IsFalse(
                    TxPluginResourceLocator.IsWellFormedRelativeResourceString(relativePath));
            }
        }
    }
}
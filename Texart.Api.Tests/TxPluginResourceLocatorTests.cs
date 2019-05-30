using System;
using NUnit.Framework;
using System.Linq;

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
            Assert.AreEqual(new [] { "plugins", "Texart.SomePlugin.dll" }, locator.AssemblySegments.ToList());
            Assert.AreEqual(TxPluginResourceLocator.OfRelativeResource("SomePath/SomeResource"), locator.RelativeResource);
            Assert.AreEqual("SomePath/SomeResource", locator.ResourcePath);
            Assert.AreEqual(new[] { "SomePath", "SomeResource" }, locator.ResourceSegments.ToList());

            Assert.IsTrue(TxPluginResourceLocator.IsWellFormedResourceLocatorUri(
                new Uri("file:///plugins/Texart.SomePlugin.dll:SomePath/SomeResource")));
        }

        [Test]
        public void AllowsEmptyAssembly()
        {
            var locator = TxPluginResourceLocator.Of("tx:///:resource/path");
            Assert.AreEqual(new TxReferenceScheme("tx"), locator.Scheme);
            Assert.AreEqual(string.Empty, locator.AssemblyPath);
            Assert.AreEqual(TxPluginResourceLocator.OfRelativeResource("resource/path"), locator.RelativeResource);
            Assert.AreEqual("resource/path", locator.ResourcePath);
            Assert.AreEqual(new[] { "resource", "path" }, locator.ResourceSegments.ToList());
        }

        [Test]
        public void AllowsEmptyResource()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugin.dll:");
            Assert.AreEqual(new TxReferenceScheme("tx"), locator.Scheme);
            Assert.AreEqual("plugin.dll", locator.AssemblyPath);
            Assert.AreEqual(TxPluginResourceLocator.OfRelativeResource(string.Empty), locator.RelativeResource);
            Assert.AreEqual(string.Empty, locator.ResourcePath);
            Assert.AreEqual(new[] { string.Empty }, locator.ResourceSegments.ToList());
        }

        [Test]
        public void AllowsEmptyAssemblyAndResource()
        {
            var locator = TxPluginResourceLocator.Of("tx:///:");
            Assert.AreEqual(new TxReferenceScheme("tx"), locator.Scheme);
            Assert.AreEqual(string.Empty, locator.AssemblyPath);
            Assert.AreEqual(new[] { string.Empty }, locator.AssemblySegments.ToList());
            Assert.AreEqual(TxPluginResourceLocator.OfRelativeResource(string.Empty), locator.RelativeResource);
            Assert.AreEqual(string.Empty, locator.ResourcePath);
            Assert.AreEqual(new[] { string.Empty }, locator.ResourceSegments.ToList());
        }

        [Test]
        public void AllowsColonsInAssemblyPath()
        {
            var locator = TxPluginResourceLocator.Of("tx:///path:to/plugin:foo.dll:resource/path");
            Assert.AreEqual(new TxReferenceScheme("tx"), locator.Scheme);
            Assert.AreEqual("path:to/plugin:foo.dll", locator.AssemblyPath);
            Assert.AreEqual(new [] { "path:to", "plugin:foo.dll" }, locator.AssemblySegments.ToList());
            Assert.AreEqual(TxPluginResourceLocator.OfRelativeResource("resource/path"), locator.RelativeResource);
            Assert.AreEqual("resource/path", locator.ResourcePath);
            Assert.AreEqual(new[] { "resource", "path" }, locator.ResourceSegments.ToList());
        }

        [Test]
        public void RejectsNoScheme() => AssertInvalidLocator("///a"); // Also tests against relative URLs

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

        [Test]
        public void RejectsAssemblyPathOnly() => AssertInvalidLocator("tx:///plugin.dll");

        private static void AssertInvalidLocator(string uri)
        {
            Assert.Throws<TxPluginResourceLocator.FormatException>(() => TxPluginResourceLocator.Of(uri));
            Assert.IsFalse(TxPluginResourceLocator.IsWellFormedResourceLocatorUri(new Uri(uri)));
        }

        [Test]
        public void AllowsWithAssembly()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugins/dummy.dll:resource/path");
            var locatorWithNewAssembly = locator.WithAssemblyPath("dummy-path/dummy2.dll");
            Assert.AreEqual(
                TxPluginResourceLocator.Of("tx:///dummy-path/dummy2.dll:resource/path"),
                locatorWithNewAssembly);
            // make sure ResourcePath backing field is not incorrectly copied
            Assert.AreEqual(locator.ResourcePath, locatorWithNewAssembly.ResourcePath);
            Assert.AreEqual(locator.ResourceSegments.ToList(), locatorWithNewAssembly.ResourceSegments.ToList());
        }

        [Test]
        public void AllowsWithAssemblyColon()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugins/dummy.dll:resource/path");
            var locatorWithNewAssembly = locator.WithAssemblyPath("dummy-path:dummy2.dll");
            Assert.AreEqual(
                TxPluginResourceLocator.Of("tx:///dummy-path:dummy2.dll:resource/path"),
                locatorWithNewAssembly);
            Assert.AreEqual("dummy-path:dummy2.dll", locatorWithNewAssembly.AssemblyPath);
            Assert.AreEqual(new[] { "dummy-path:dummy2.dll" }, locatorWithNewAssembly.AssemblySegments.ToList());
            Assert.AreEqual(locator.RelativeResource, locatorWithNewAssembly.RelativeResource);
            // make sure ResourcePath backing field is not incorrectly copied
            Assert.AreEqual(locator.ResourcePath, locatorWithNewAssembly.ResourcePath);
            Assert.AreEqual( locator.ResourceSegments.ToList(), locatorWithNewAssembly.ResourceSegments.ToList());
        }

        [Test]
        public void AllowsWithAssemblyEscaped()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugins/dummy.dll:resource/path");
            var locatorWithNewAssembly = locator.WithAssemblyPath("dummy%20-path:dummy%4A2.dll");
            // TODO: Investigate why .NET Core parses this URI differently from other implementations (including other platforms)
            // new Uri("tx:///dummy%20-path:dummy%4A2.dll:resource/path", UriKind.Absolute);
            // Workaround is to help .NET Core out a little
            var expectedUriBuilder = new UriBuilder("tx", string.Empty)
            {
                Path = "/dummy%20-path:dummy%4A2.dll:resource/path"
            };
            var expected = TxPluginResourceLocator.Of(expectedUriBuilder.Uri);

            Assert.AreEqual(expected, locatorWithNewAssembly);
            Assert.AreEqual("dummy%20-path:dummyJ2.dll", locatorWithNewAssembly.AssemblyPath);
            Assert.AreEqual(new[] { "dummy%20-path:dummyJ2.dll" }, locatorWithNewAssembly.AssemblySegments.ToList());
            Assert.AreEqual(locator.RelativeResource, locatorWithNewAssembly.RelativeResource);
            // make sure ResourcePath backing field is not incorrectly copied
            Assert.AreEqual(locator.ResourcePath, locatorWithNewAssembly.ResourcePath);
            Assert.AreEqual(locator.ResourceSegments.ToList(), locatorWithNewAssembly.ResourceSegments.ToList());
        }

        [Test]
        public void RejectsWithAssemblyQuery()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugins/dummy.dll:resource/path");
            var ex = Assert.Throws<TxPluginResourceLocator.FormatException>(
                () => locator.WithAssemblyPath("dummy2.dll?key=value"));
            Assert.IsTrue(ex.Message.Contains("query"));
        }

        [Test]
        public void RejectsWithAssemblyFragment()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugins/dummy.dll:resource/path");
            var ex = Assert.Throws<TxPluginResourceLocator.FormatException>(
                () => locator.WithAssemblyPath("dummy2.dll#fragment"));
            Assert.IsTrue(ex.Message.Contains("fragment"));
        }

        [Test]
        public void AllowsWithSchemeValid()
        {
            var locator = TxPluginResourceLocator.Of("dummy:///plugins/dummy.dll:resource/path");
            var expected = TxPluginResourceLocator.Of("tx:///plugins/dummy.dll:resource/path");
            Assert.AreEqual(expected, locator.WithScheme("tx"));
            Assert.AreEqual(expected, locator.WithScheme(TxReferenceScheme.Tx));
            // make sure ResourcePath backing field is not incorrectly copied
            Assert.AreEqual(expected.ResourcePath, locator.WithScheme(TxReferenceScheme.Tx).ResourcePath);
            Assert.AreEqual(expected.ResourceSegments.ToList(), locator.WithScheme(TxReferenceScheme.Tx).ResourceSegments.ToList());
        }

        [Test]
        public void RejectsWithSchemeInvalid()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugins/dummy.dll:resource/path");
            var ex = Assert.Throws<TxPluginResourceLocator.FormatException>(
                () => locator.WithScheme("invalid/scheme"));
            Assert.IsTrue(ex.Message.Contains("scheme"));
        }

        [Test]
        public void AllowsWithRelativeResourceValid()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugins/dummy.dll:resource/path");
            var expected = TxPluginResourceLocator.Of("tx:///plugins/dummy.dll:/different/path");
            Assert.AreEqual(expected, locator.WithRelativeResource("/different/path"));
            Assert.AreEqual(
                expected,
                locator.WithRelativeResource(TxPluginResourceLocator.OfRelativeResource("/different/path")));
            // make sure ResourcePath backing field is not incorrectly copied
            Assert.AreEqual(expected.ResourcePath, locator.WithRelativeResource("/different/path").ResourcePath);
            Assert.AreEqual(expected.ResourceSegments.ToList(), locator.WithRelativeResource("/different/path").ResourceSegments.ToList());
        }

        [Test]
        public void AllowsWithRelativeResourceEmpty()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugins/dummy.dll:resource/path");
            var expected = TxPluginResourceLocator.Of("tx:///plugins/dummy.dll:");
            Assert.AreEqual(expected, locator.WithRelativeResource(string.Empty));
            Assert.AreEqual(expected, locator.WithoutRelativeResource);
        }

        [Test]
        public void RejectsWithRelativeResourceQuery()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugins/dummy.dll:resource/path");
            var ex = Assert.Throws<TxPluginResourceLocator.FormatException>(
                () => locator.WithRelativeResource("resource?key=value"));
            Assert.IsTrue(ex.Message.Contains("query"));
        }

        [Test]
        public void RejectsWithRelativeResourceFragment()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugins/dummy.dll:resource/path");
            var ex = Assert.Throws<TxPluginResourceLocator.FormatException>(
                () => locator.WithRelativeResource("resource#fragment"));
            Assert.IsTrue(ex.Message.Contains("fragment"));
        }

        [Test]
        public void AllowsEscapedUris()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugin%20.dll:hello/%20wor%3Ald");
            Assert.AreEqual(new TxReferenceScheme("tx"), locator.Scheme);
            Assert.AreEqual("plugin%20.dll", locator.AssemblyPath);
            Assert.AreEqual(TxPluginResourceLocator.OfRelativeResource("hello/%20wor%3Ald"), locator.RelativeResource);
            Assert.AreEqual("hello/%20wor%3Ald", locator.ResourcePath);
            Assert.AreEqual(new[] { "hello", "%20wor%3Ald" }, locator.ResourceSegments.ToList());
        }

        [Test]
        public void HasCorrectUriRepresentation()
        {
            var locator = TxPluginResourceLocator.Of("tx:///plugin%20.dll:hello/%20wor%3Ald");
            Assert.AreEqual(
                new Uri("tx:///plugin%20.dll:hello/%20wor%3Ald", UriKind.Absolute),
                locator.AsUri);
        }

        [Test]
        public void HasCorrectEquality()
        {
            var locator1 = TxPluginResourceLocator.Of("tx:///plugin%20.dll:hello/%20wor%3Ald");
            var locator2 = TxPluginResourceLocator.Of("tx:///plugin%20.dll:hello/%20wor%3Ald");

            Assert.IsTrue(locator1 == locator2);
            Assert.IsTrue(locator2 == locator1);
            Assert.IsFalse(locator1 != locator2);
            Assert.IsFalse(locator2 != locator1);
        }

        [Test]
        public void HasCorrectInequalityWithScheme()
        {
            var locator1 = TxPluginResourceLocator.Of("file:///plugin%20.dll:hello/%20wor%3Ald");
            var locator2 = TxPluginResourceLocator.Of("tx:///plugin%20.dll:hello/%20wor%3Ald");

            Assert.IsFalse(locator1 == locator2);
            Assert.IsFalse(locator2 == locator1);
            Assert.IsTrue(locator1 != locator2);
            Assert.IsTrue(locator2 != locator1);
        }

        [Test]
        public void HasCorrectInequalityWithAssemblyPath()
        {
            var locator1 = TxPluginResourceLocator.Of("tx:///hello/world.dll:hello/%20wor%3Ald");
            var locator2 = TxPluginResourceLocator.Of("tx:///plugin%20.dll:hello/%20wor%3Ald");

            Assert.IsFalse(locator1 == locator2);
            Assert.IsFalse(locator2 == locator1);
            Assert.IsTrue(locator1 != locator2);
            Assert.IsTrue(locator2 != locator1);
        }

        [Test]
        public void HasCorrectInequalityWithResourcePath()
        {
            var locator1 = TxPluginResourceLocator.Of("tx:///plugin%20.dll:world/%20hel%3Alo");
            var locator2 = TxPluginResourceLocator.Of("tx:///plugin%20.dll:hello/%20wor%3Ald");

            Assert.IsFalse(locator1 == locator2);
            Assert.IsFalse(locator2 == locator1);
            Assert.IsTrue(locator1 != locator2);
            Assert.IsTrue(locator2 != locator1);
        }

        [Test]
        public void HasCorrectHashCodes()
        {
            var locator1 = TxPluginResourceLocator.Of("tx:///plugin%20.dll:world/%20hel%3Alo");
            var locator2 = TxPluginResourceLocator.Of("tx:///plugin%20.dll:hello/%20wor%3Ald");
            var locator3 = TxPluginResourceLocator.Of("tx:///plugin%20.dll:world/%20hel%3Alo");
            var locator4 = TxPluginResourceLocator.Of("tx:///plugin%20.dll:hello/%20wor%3Ald");

            Assert.AreEqual(locator1.GetHashCode(), locator3.GetHashCode());
            Assert.AreEqual(locator2.GetHashCode(), locator4.GetHashCode());
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
                Assert.AreEqual(new[] { "SomePath", "SomeResource" }, relative.ResourceSegments.ToList());
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
                Assert.AreEqual(new[] { string.Empty }, relative.ResourceSegments.ToList());
                Assert.AreEqual(
                    TxPluginResourceLocator.Of("tx:///assembly.dll:").RelativeResource,
                    relative);
            }

            [Test]
            public void AllowsEscaped()
            {
                var relative = TxPluginResourceLocator.OfRelativeResource("/resource%20/pa%3Ath");
                Assert.AreEqual("/resource%20/pa%3Ath", relative.ResourcePath);
                Assert.AreEqual(new[] { string.Empty, "resource%20", "pa%3Ath" }, relative.ResourceSegments.ToList());
                Assert.AreEqual(
                    TxPluginResourceLocator.Of("tx:///assembly.dll:/resource%20/pa%3Ath").RelativeResource,
                    relative);
            }

            [Test]
            public void AllowsLeadingSlash()
            {
                var relative = TxPluginResourceLocator.OfRelativeResource("/resource/path");
                Assert.AreEqual("/resource/path", relative.ResourcePath);
                Assert.AreEqual(new[] { string.Empty, "resource", "path" }, relative.ResourceSegments.ToList());
                Assert.AreEqual(
                    TxPluginResourceLocator.Of("tx:///assembly.dll:/resource/path").RelativeResource,
                    relative);
            }

            [Test]
            public void AllowsEmptyWithLeadingSlash()
            {
                var relative = TxPluginResourceLocator.OfRelativeResource("/");
                Assert.AreEqual("/", relative.ResourcePath);
                Assert.AreEqual(new[] { string.Empty, string.Empty }, relative.ResourceSegments.ToList());
            }

            [Test]
            public void PreservesLeadingSlashes()
            {
                var relative = TxPluginResourceLocator.OfRelativeResource("///hello");
                Assert.AreEqual("///hello", relative.ResourcePath);
                Assert.AreEqual(
                    new[] { string.Empty, string.Empty, string.Empty, "hello" },
                    relative.ResourceSegments.ToList());
            }

            [Test]
            public void RejectsColon() => AssertInvalidRelativeResource("resource/pa:th");

            [Test]
            public void RejectsQuery() => AssertInvalidRelativeResource("c?hello=world");

            [Test]
            public void RejectsEmptyQuery() => AssertInvalidRelativeResource("c?");

            [Test]
            public void RejectsFragment() => AssertInvalidRelativeResource("c#hello");

            [Test]
            public void RejectsEmptyFragment() => AssertInvalidRelativeResource("c#");

            private static void AssertInvalidRelativeResource(string relativePath)
            {
                Assert.Throws<TxPluginResourceLocator.FormatException>(
                    () => TxPluginResourceLocator.OfRelativeResource(relativePath));
                Assert.IsFalse(
                    TxPluginResourceLocator.IsWellFormedRelativeResourceString(relativePath));
            }
        }
    }
}
using System;
using NUnit.Framework;
using Texart.Plugins.Scripting;

namespace Texart.Plugins.Tests.Scripting
{
    internal class ReferenceSchemeTests
    {
        [Test]
        public void AllowsSimple() => AssertValidScheme("hello");

        [Test]
        public void AllowsSingleCharacter() => AssertValidScheme("h");

        [Test]
        public void AllowsTwoCharacters() => AssertValidScheme("he");

        [Test]
        public void AllowsNumbers() => AssertValidScheme("h0t");

        [Test]
        public void AllowsHyphen() => AssertValidScheme("hello-world");

        private static void AssertValidScheme(string scheme) =>
            Assert.DoesNotThrow(() => new ReferenceScheme(scheme));

        [Test]
        public void RejectsEmpty() => AssertInvalidScheme(string.Empty);

        [Test]
        public void RejectsWhitespace() {
            AssertInvalidScheme("hello world");
            AssertInvalidScheme("hello\tworld");
            AssertInvalidScheme("\nhello");
            AssertInvalidScheme("\r\nhello");
            AssertInvalidScheme("hello\n");
            AssertInvalidScheme("hello\r\n");
            AssertInvalidScheme("hello\n\r");
            AssertInvalidScheme("hello\nworld");
        }

        [Test]
        public void RejectsHyphenAtStart() => AssertInvalidScheme("-hello");

        [Test]
        public void RejectsOnlyHyphen() => AssertInvalidScheme("-");

        [Test]
        public void RejectsNumberAtStart() => AssertInvalidScheme("4hello");

        [Test]
        public void RejectsOnlyNumber() => AssertInvalidScheme("0");

        [Test]
        public void RejectsUnderscore() => AssertInvalidScheme("hel_lo");

        [Test]
        public void RejectsColon() => AssertInvalidScheme("he:lo");

        [Test]
        public void RejectsColonAtEnd() => AssertInvalidScheme("hello:");

        private static void AssertInvalidScheme(string scheme)
        {
            var ex = Assert.Throws<ArgumentException>(() => new ReferenceScheme(scheme));
            Assert.IsTrue(
                ex.Message.StartsWith("scheme is not valid"),
                "Exception was thrown but not because of invalid scheme");
        }
    }
}
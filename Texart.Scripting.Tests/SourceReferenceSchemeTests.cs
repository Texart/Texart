using System;
using NUnit.Framework;

namespace Texart.Scripting.Tests
{
    internal static class SourceReferenceSchemeTests
    {
        private class AllowanceTests
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

            private void AssertValidScheme(string scheme) =>
                Assert.DoesNotThrow(() => new SourceReferenceScheme(scheme));
        }

        private class RejectionTests
        {
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

            private void AssertInvalidScheme(string scheme)
            {
                var ex = Assert.Throws<ArgumentException>(() => new SourceReferenceScheme(scheme));
                Assert.IsTrue(
                    ex.Message.StartsWith("scheme is not valid"),
                    "Exception was thrown but not because of invalid scheme");
            }
        }
    }
}
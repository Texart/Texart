using System;
using NUnit.Framework;

namespace Texart.Scripting.Tests
{
    internal static class SourceReferenceSchemeTests
    {
        private class RejectionTests
        {
            [Test]
            public void RejectsEmpty() =>
                AssertInvalidScheme(string.Empty);

            [Test]
            public void RejectsWhitespace() =>
                AssertInvalidScheme("hello world");

            [Test]
            public void RejectsHyphenAtStart() =>
                AssertInvalidScheme("-hello");

            [Test]
            public void RejectsNumberAtStart() =>
                AssertInvalidScheme("4hello");

            [Test]
            public void RejectsUnderscore() =>
                AssertInvalidScheme("hel_lo");

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
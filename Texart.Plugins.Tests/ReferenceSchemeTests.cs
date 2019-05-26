using System;
using NUnit.Framework;

namespace Texart.Plugins.Tests
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

        [Test]
        public void LowerCasesScheme() =>
            Assert.AreEqual("file", new ReferenceScheme("FiLe").Scheme);

        private static void AssertValidScheme(string scheme)
        {
            ReferenceScheme instance = null;
            void CreateInstance()
            {
                instance = new ReferenceScheme(scheme);
            }
            Assert.DoesNotThrow(CreateInstance);
            Assert.AreEqual(scheme, instance.Scheme);

            void CheckMatches()
            {
                Assert.IsTrue(instance.Matches($"{scheme}://hello:world"));
                Assert.IsTrue(instance.Matches($"{scheme}://"));
                Assert.IsFalse(instance.Matches(scheme));
                Assert.IsFalse(instance.Matches($"{scheme}hello://world"));
            }
            void CheckPrefix()
            {
                Assert.AreEqual($"{scheme}://", instance.SchemePrefix);
                Assert.AreEqual($"{scheme}://hello/world", instance.Prefix("hello/world"));
            }
            void CheckNormalizePath()
            {
                Assert.AreEqual(instance.NormalizePath($"{scheme}://hello/world"), "hello/world");
            }

            CheckMatches();
            CheckPrefix();
            CheckNormalizePath();
        }

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

        [Test]
        public void RejectsDot() => AssertInvalidScheme("hello.world");

        [Test]
        public void RejectsPlus() => AssertInvalidScheme("http+https");

        private static void AssertInvalidScheme(string scheme)
        {
            var ex = Assert.Throws<ArgumentException>(() => new ReferenceScheme(scheme));
            Assert.IsTrue(
                ex.Message.StartsWith("scheme is not valid"),
                "Exception was thrown but not because of invalid scheme");
        }

        [Test]
        public void HasCorrectEquality()
        {
            var file1 = new ReferenceScheme("file");
            var file2 = new ReferenceScheme("file");

            Assert.IsTrue(file1 == file2);
            Assert.IsTrue(file2 == file1);
            Assert.IsFalse(file1 != file2);
            Assert.IsFalse(file2 != file1);
        }

        [Test]
        public void HasCorrectInequality()
        {
            var file = new ReferenceScheme("file");
            var https = new ReferenceScheme("https");

            Assert.IsFalse(file == https);
            Assert.IsFalse(https == file);
            Assert.IsTrue(file != https);
            Assert.IsTrue(https != file);
        }

        [Test]
        public void HasCorrectHashCodes()
        {
            var file1 = new ReferenceScheme("file");
            var file2 = new ReferenceScheme("FILE");
            var https1 = new ReferenceScheme("https");
            var https2 = new ReferenceScheme("HTTPS");

            Assert.AreEqual(file1.GetHashCode(), file2.GetHashCode());
            Assert.AreEqual(https1.GetHashCode(), https2.GetHashCode());
        }
    }
}
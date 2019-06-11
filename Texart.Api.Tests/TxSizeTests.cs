using NUnit.Framework;
using SkiaSharp;

namespace Texart.Api.Tests
{
    [TestFixture]
    internal class TxSizeTests
    {
        [Test]
        public void AllowsParsingInt()
        {
            TxSize.TryParseI("1000x1000", out var size);
            Assert.AreEqual(new SKSizeI(1000, 1000), size);
        }

        [Test]
        public void AllowsParsingIntZeros()
        {
            TxSize.TryParseI("0x0", out var size);
            Assert.AreEqual(new SKSizeI(0, 0), size);
        }

        [Test]
        public void AllowsParsingIntWithLeadingWhitespace()
        {
            TxSize.TryParseI("  1000x1000", out var size);
            Assert.AreEqual(new SKSizeI(1000, 1000), size);
        }

        [Test]
        public void AllowsParsingIntWithTrailingWhitespace()
        {
            TxSize.TryParseI("1000x1000  ", out var size);
            Assert.AreEqual(new SKSizeI(1000, 1000), size);
        }

        [Test]
        public void RejectsParsingIntWithWhitespaceInBetween()
        {
            Assert.IsFalse(TxSize.TryParseI("1000 x1000", out _));
            Assert.IsFalse(TxSize.TryParseI("1000x 1000", out _));
            Assert.IsFalse(TxSize.TryParseI("1000 x 1000", out _));
        }

        [Test]
        public void RejectsParsingIntEmpty()
        {
            Assert.IsFalse(TxSize.TryParseI(string.Empty, out _));
        }

        [Test]
        public void RejectsParsingIntWithNegative()
        {
            Assert.IsFalse(TxSize.TryParseI("-1x1", out _));
            Assert.IsFalse(TxSize.TryParseI("1x-1", out _));
        }

        [Test]
        public void RejectsParsingIntWithoutSeparator()
        {
            Assert.IsFalse(TxSize.TryParseI("1000 1000", out _));
        }

        [Test]
        public void RejectsParsingIntWithMultipleSeparators()
        {
            Assert.IsFalse(TxSize.TryParseI("1000x1000x1000", out _));
        }

        [Test]
        public void RejectsParsingHexInt()
        {
            Assert.IsFalse(TxSize.TryParseI("0x1x0x1", out _));
        }

        [Test]
        public void RejectsParsingIntWithDecimalPoint()
        {
            Assert.IsFalse(TxSize.TryParseI("1000.x1000.", out _));
            Assert.IsFalse(TxSize.TryParseI("1000.0x1000.0", out _));
        }
    }
}
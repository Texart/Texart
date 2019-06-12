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

        [Test]
        public void AllowsParsingFloat()
        {
            TxSize.TryParse("1000x1000", out var size);
            Assert.AreEqual(new SKSize(1000, 1000), size);

            TxSize.TryParse("1000.0x1000.0", out var size2);
            Assert.AreEqual(new SKSize(1000, 1000), size2);
        }

        [Test]
        public void AllowsParsingFloatZeros()
        {
            TxSize.TryParse("0x0", out var size);
            Assert.AreEqual(new SKSize(0, 0), size);

            TxSize.TryParse("0.0x0.0", out var size2);
            Assert.AreEqual(new SKSize(0, 0), size2);
        }

        [Test]
        public void AllowsParsingFloatWithLeadingWhitespace()
        {
            TxSize.TryParse("  1000.0x1000", out var size);
            Assert.AreEqual(new SKSize(1000, 1000), size);
        }

        [Test]
        public void AllowsParsingFloatWithTrailingWhitespace()
        {
            TxSize.TryParse("1000x1000.0  ", out var size);
            Assert.AreEqual(new SKSize(1000, 1000), size);
        }

        [Test]
        public void RejectsParsingFloatWithWhitespaceInBetween()
        {
            Assert.IsFalse(TxSize.TryParse("1000 x1000", out _));
            Assert.IsFalse(TxSize.TryParse("1000x 1000", out _));
            Assert.IsFalse(TxSize.TryParse("1000 x 1000", out _));
            Assert.IsFalse(TxSize.TryParse("1000.0 x1000", out _));
            Assert.IsFalse(TxSize.TryParse("1000.0x 1000", out _));
            Assert.IsFalse(TxSize.TryParse("1000 x 1000.0", out _));
        }

        [Test]
        public void RejectsParsingFloatEmpty()
        {
            Assert.IsFalse(TxSize.TryParse(string.Empty, out _));
        }

        [Test]
        public void RejectsParsingFloatWithNegative()
        {
            Assert.IsFalse(TxSize.TryParse("-1x1", out _));
            Assert.IsFalse(TxSize.TryParse("1x-1", out _));
            Assert.IsFalse(TxSize.TryParse("-1.0x1", out _));
            Assert.IsFalse(TxSize.TryParse("1.0x-1", out _));
        }

        [Test]
        public void RejectsParsingFloatWithoutSeparator()
        {
            Assert.IsFalse(TxSize.TryParse("1000 1000.0", out _));
        }

        [Test]
        public void RejectsParsingFloatWithMultipleSeparators()
        {
            Assert.IsFalse(TxSize.TryParse("1000x1000.0x1000", out _));
        }

        [Test]
        public void RejectsParsingExponentFloat()
        {
            Assert.IsFalse(TxSize.TryParse("1e3x1000", out _));
        }

        [Test]
        public void AllowsParsingFloatWithTrailingDecimalPoint()
        {
            TxSize.TryParse("1000.x1000.", out var size);
            Assert.AreEqual(new SKSize(1000, 1000), size);
        }
    }
}
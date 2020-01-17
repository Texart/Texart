using System;
using System.Globalization;
using SkiaSharp;

#nullable enable

namespace Texart.Api
{
    /// <summary>
    /// Helpers for size representation.
    /// </summary>
    /// <seealso cref="SKSize"/>
    /// <seealso cref="SKSizeI"/>
    public static class TxSize
    {
        /// <summary>
        /// Tries to parse <paramref name="text"/> into a <see cref="SKSizeI"/>.
        /// The format of <paramref name="text"/> is expected to be <c>$"{width}x{height}"</c>.
        /// Both width and height must be <c>>= 0</c>.
        /// Whitespace is not allowed between the numbers.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="size">The parsed <see cref="SKSizeI"/>.</param>
        /// <returns><c>true</c> if parsing succeeded, <c>false</c> otherwise.</returns>
        public static bool TryParseI(ReadOnlySpan<char> text, out SKSizeI size)
        {
            var separatorIndex = text.IndexOf(SizeSeparator);
            if (separatorIndex < 0)
            {
                size = default;
                return false;
            }

            var formatProvider = NumberFormatInfo.InvariantInfo;
            // No whitespaces, leading sign - which are usually default for int.TryParse
            const NumberStyles commonNumberStyles = NumberStyles.None;

            const NumberStyles widthNumberStyles = commonNumberStyles | NumberStyles.AllowLeadingWhite;
            var widthString = text.Slice(0, separatorIndex);
            if (!int.TryParse(widthString, widthNumberStyles, formatProvider, out var width) || width < 0)
            {
                size = default;
                return false;
            }

            var heightString = text.Slice(separatorIndex + 1);
            const NumberStyles heightNumberStyles = commonNumberStyles | NumberStyles.AllowTrailingWhite;
            if (!int.TryParse(heightString, heightNumberStyles, formatProvider, out var height) || height < 0)
            {
                size = default;
                return false;
            }

            size = new SKSizeI(width, height);
            return true;
        }

        /// <summary>
        /// Tries to parse <paramref name="text"/> into a <see cref="SKSize"/>.
        /// The format of <paramref name="text"/> is expected to be <c>$"{width}x{height}"</c>.
        /// Both width and height must be <c>>= 0</c>.
        /// Whitespace is not allowed between the numbers.
        /// Exponents are not allowed (i.e. in the form <c>1e3</c>).
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="size">The parsed <see cref="SKSize"/>.</param>
        /// <returns><c>true</c> if parsing succeeded, <c>false</c> otherwise.</returns>
        public static bool TryParse(ReadOnlySpan<char> text, out SKSize size)
        {
            var separatorIndex = text.IndexOf(SizeSeparator);
            if (separatorIndex < 0)
            {
                size = default;
                return false;
            }

            var formatProvider = NumberFormatInfo.InvariantInfo;
            // No whitespaces, leading sign, exponents - which are usually default for float.TryParse
            const NumberStyles commonNumberStyles = NumberStyles.AllowDecimalPoint;

            const NumberStyles widthNumberStyles = commonNumberStyles | NumberStyles.AllowLeadingWhite;
            var widthString = text.Slice(0, separatorIndex);
            if (!float.TryParse(widthString, widthNumberStyles, formatProvider, out var width) || width < 0)
            {
                size = default;
                return false;
            }

            const NumberStyles heightNumberStyles = commonNumberStyles | NumberStyles.AllowTrailingWhite;
            var heightString = text.Slice(separatorIndex + 1);
            if (!float.TryParse(heightString, heightNumberStyles, formatProvider, out var height) || height < 0)
            {
                size = default;
                return false;
            }

            size = new SKSize(width, height);
            return true;
        }

        /// <summary>
        /// The separator between the width and the height in a size string.
        /// </summary>
        private const char SizeSeparator = 'x';
    }
}
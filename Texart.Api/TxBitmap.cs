using System;
using System.Collections.Generic;
using System.Diagnostics;
using SkiaSharp;

namespace Texart.Api
{
    /// <summary>
    /// Helpers for image bitmaps.
    /// </summary>
    /// <see cref="SKBitmap"/>
    public static class TxBitmap
    {
        /// <summary>
        /// Returns every possible values for <see cref="ITextBitmapGenerator.PixelSamplingRatio"/>
        /// for the given image bitmap. This value depends only on the bitmap size, not it's pixel
        /// data.
        /// <see cref="GetPerfectPixelRatios(int, int)"/>
        /// </summary>
        /// <param name="bitmap">The bitmap whose dimensions to check.</param>
        /// <returns>Every possible factor.</returns>
        public static IEnumerable<int> GetPerfectPixelRatios(SKBitmap bitmap) =>
            GetPerfectPixelRatios(bitmap.Width, bitmap.Height);

        /// <summary>
        /// Returns every possible values for <see cref="ITextBitmapGenerator.PixelSamplingRatio"/>
        /// for the given dimensions.
        /// </summary>
        /// <param name="width">The width of bitmap to check.</param>
        /// <param name="height">The height of bitmap to check.</param>
        /// <returns>Every possible factor.</returns>
        public static IEnumerable<int> GetPerfectPixelRatios(int width, int height)
        {
            // The idea is to get every common factor between the width and the height
            Debug.Assert(width > 0);
            Debug.Assert(height > 0);

            // All common factors are <= the greatest common factor by definition.
            // So listing all factors of the gcd is sufficient.
            var gcd = Gcd(width, height);
            Debug.Assert(gcd >= 1);

            var checkMax = (int)Math.Sqrt(gcd);
            for (var possibleFactor = checkMax; possibleFactor >= 1; --possibleFactor)
            {
                if (gcd % possibleFactor != 0)
                {
                    continue;
                }
                var factor = possibleFactor;
                yield return factor;
                var complementaryFactor = gcd / factor;
                // don't yield the same number twice
                if (complementaryFactor != factor)
                {
                    yield return complementaryFactor;
                }
            }

            int Gcd(int a, int b)
            {
                // Generic Euclidean GCD algorithm
                while (b != 0)
                {
                    var temp = b;
                    b = a % b;
                    a = temp;
                }
                return a;
            }
        }
    }
}

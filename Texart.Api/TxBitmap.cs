using System;
using System.Collections.Generic;
using System.Diagnostics;
using SkiaSharp;

namespace Texart.Api
{
    /// <summary>
    /// Helpers for image bitmaps. Here are some concepts that you should be familiar with:
    ///
    ///   * Sampling factor: The width (or height) of a square grid of pixels from a source <see cref="SKBitmap"/>
    ///         that an <see cref="ITxTextBitmapGenerator"/> maps to exactly one <see cref="char"/> in <see cref="ITxTextBitmap"/>.
    ///         That is, an <see cref="ITxTextBitmapGenerator"/>, <c>generator</c>, using a <i>sampling factor</i>, <c>factor</c>,
    ///         will produce an <see cref="ITxTextBitmap"/>, <c>textBitmap</c> from an <see cref="SKBitmap"/>, <c>bitmap</c>,
    ///         such that <c>textBitmap.Width * factor == bitmap.Width</c> and <c>textBitmap.Height * factor == bitmap.Height</c>.
    ///         For example, a <i>sampling factor</i> of 3 means that a 3x3 square (of 9 pixels) in a bitmap will be used to come up
    ///         with exactly one output <see cref="char"/>. Consequently, a <i>sampling factor</i> of 1 maps every pixel to one
    ///         <see cref="char"/> and the size of the generated <c>textBitmap</c> and source <c>bitmap</c> are equal.
    ///
    ///   * Perfect sampling factor: A <i>sampling factor</i>, <c>factor</c> that can break up a whole <see cref="SKBitmap"/> completely
    ///         into a grid of squares of with side length of <c>factor</c>. That is, both the bitmap's height and width is divisible by
    ///         <c>factor</c>. A perfect <i>sampling factor</i> ensures that the same number of pixels are used to determine every
    ///         <see cref="char"/> in <see cref="ITxTextBitmap"/>. A perfect <i>sampling factor</i> also implies that the output image
    ///         maintains the source aspect ratio without cropping the image. There is always at least one perfect <i>sampling factor</i>,
    ///         <c>1</c>.
    /// </summary>
    /// <seealso cref="SKBitmap"/>
    public static class TxBitmap
    {
        /// <summary>
        /// A tag representing the strategy to determine if a sampling factor is <i>close</i> to achieving a
        /// provided target size.
        /// </summary>
        /// <seealso cref="BestSamplingFactor"/>
        public enum SamplingFactorCloseness
        {
            /// <summary>
            /// Matches a sampling factor that produces the largest dimension which is smaller than the target size
            /// (in both width and height). If no sampling size produces a dimension smaller than the target size,
            /// then the largest sampling size is returned (which produces the smallest possible dimension).
            ///
            /// That is, the output image will be of maximal size while still being smaller than the target size,
            /// and maintaining a perfect sampling factor (see <see cref="TxBitmap.IsPerfectSamplingFactor"/>).
            /// </summary>
            SmallerThanTarget,
            /// <summary>
            /// Matches a sampling factor that produces the smallest dimension which is larger than the target size
            /// (in both width and height). If no sampling size produces a dimension larger than the target size,
            /// then the smallest sampling size is returned (which produces the largest possible dimension).
            ///
            /// That is, the output image will be of minimal size while still being larger than the target size,
            /// and maintaining a perfect sampling factor (see <see cref="TxBitmap.IsPerfectSamplingFactor"/>).
            /// </summary>
            LargerThanTarget,
            /// <summary>
            /// Matches a sampling factor that produces the dimensions with the smallest possible euclidean distance
            /// to the target size.
            ///
            /// That is, the output image will be of a size <i>close</i> to the target size while maintaining a
            /// perfect sampling factor (see <see cref="TxBitmap.IsPerfectSamplingFactor"/>).
            /// </summary>
            EuclideanDistanceToTarget
        }
        /// <summary>
        /// Finds the best sampling factor to use that will cause a bitmap of size <paramref name="sourceSize"/>
        /// to produce an output text bitmap that is <i>close</i> in size to <paramref name="targetSize"/>. The
        /// returned sampling factor is guaranteed to be perfect (see <see cref="IsPerfectSamplingFactor"/>).
        /// character in the output bitmap is assumed to take up a square of <paramref name="fontSize"/> pixels.
        /// <i>Closeness</i> is defined by <paramref name="closeness"/>.
        /// </summary>
        /// <param name="sourceSize">The size of the input <see cref="SKBitmap"/>.</param>
        /// <param name="targetSize">The desired size of the output <see cref="SKBitmap"/> of characters.</param>
        /// <param name="fontSize">The space (in pixels) used by each character in the output bitmap.</param>
        /// <param name="closeness">
        ///     The strategy used to determine how close a sampling factor is to determining the target
        ///     output size.
        /// </param>
        /// <returns>
        ///     A perfect sampling factor that is the "best" for an output bitmap of
        ///     <paramref name="targetSize"/>, where "best" is defined by <paramref name="closeness"/>.
        /// </returns>
        /// <seealso cref="SamplingFactorCloseness"/>
        public static int BestSamplingFactor(
            SKSizeI sourceSize, SKSizeI targetSize,
            float fontSize,
            SamplingFactorCloseness closeness = SamplingFactorCloseness.EuclideanDistanceToTarget)
        {
            int? maybeIdealSamplingFactor = null;
            foreach (var samplingFactor in PerfectSamplingFactors(sourceSize.Width, sourceSize.Height))
            {
                var expectedSize = ExpectedOutputSize(sourceSize, fontSize, samplingFactor);
                if (!maybeIdealSamplingFactor.HasValue)
                {
                    maybeIdealSamplingFactor = samplingFactor;
                    continue;
                }

                maybeIdealSamplingFactor = closeness switch
                {
                    SamplingFactorCloseness.SmallerThanTarget => BetterFactorForSmaller(
                        sourceSize, targetSize,
                        fontSize,
                        maybeIdealSamplingFactor.Value, samplingFactor),
                    SamplingFactorCloseness.LargerThanTarget => BetterFactorForLarger(
                        sourceSize, targetSize,
                        fontSize,
                        maybeIdealSamplingFactor.Value, samplingFactor),
                    SamplingFactorCloseness.EuclideanDistanceToTarget => BetterFactorForEuclideanDistance(
                        sourceSize, targetSize,
                        fontSize,
                        maybeIdealSamplingFactor.Value, samplingFactor),
                    _ => throw new InvalidOperationException("Unreachable case")
                };
            }
            // there is at least one sampling factor at least
            Debug.Assert(maybeIdealSamplingFactor.HasValue);
            return maybeIdealSamplingFactor.Value;

            static int BetterFactorForSmaller(SKSizeI source, SKSizeI target, float font, int lhs, int rhs)
            {
                if (lhs == rhs)
                {
                    return lhs;
                }

                var left = ExpectedOutputSize(source, font, lhs);
                var right = ExpectedOutputSize(source, font, rhs);

                var leftIsSmaller = IsSmallerOrEqual(left, target);
                var rightIsSmaller = IsSmallerOrEqual(right, target);

                if (leftIsSmaller && !rightIsSmaller)
                {
                    return lhs;
                }

                if (!leftIsSmaller && rightIsSmaller)
                {
                    return rhs;
                }

                var leftDistance = EuclideanDistanceSquared(left, target);
                var rightDistance = EuclideanDistanceSquared(right, target);
                Debug.Assert(leftIsSmaller == rightIsSmaller);
                // Whatever is closest to the target size
                return leftDistance < rightDistance ? lhs : rhs;
            }

            static int BetterFactorForLarger(SKSizeI source, SKSizeI target, float font, int lhs, int rhs)
            {
                if (lhs == rhs)
                {
                    return lhs;
                }

                var left = ExpectedOutputSize(source, font, lhs);
                var right = ExpectedOutputSize(source, font, rhs);

                var leftIsGreater = IsGreaterOrEqual(left, target);
                var rightIsGreater = IsGreaterOrEqual(right, target);

                if (leftIsGreater && !rightIsGreater)
                {
                    return lhs;
                }

                if (!leftIsGreater && rightIsGreater)
                {
                    return rhs;
                }

                var leftDistance = EuclideanDistanceSquared(left, target);
                var rightDistance = EuclideanDistanceSquared(right, target);
                Debug.Assert(leftIsGreater == rightIsGreater);
                // Whatever is closest to the target size
                return leftDistance < rightDistance ? lhs : rhs;
            }

            static int BetterFactorForEuclideanDistance(SKSizeI source, SKSizeI target, float font, int lhs, int rhs)
            {
                if (lhs == rhs)
                {
                    return lhs;
                }

                var left = ExpectedOutputSize(source, font, lhs);
                var right = ExpectedOutputSize(source, font, rhs);

                var leftDistance = EuclideanDistanceSquared(left, target);
                var rightDistance = EuclideanDistanceSquared(right, target);
                return leftDistance < rightDistance ? lhs : rhs;
            }

            static bool IsSmallerOrEqual(SKSizeI lhs, SKSizeI rhs) =>
                lhs.Width <= rhs.Width && lhs.Height <= rhs.Height;

            static bool IsGreaterOrEqual(SKSizeI lhs, SKSizeI rhs) =>
                lhs.Width >= rhs.Width && lhs.Height >= rhs.Height;

            static int EuclideanDistanceSquared(SKSizeI lhs, SKSizeI rhs)
            {
                var deltaX = lhs.Width - rhs.Width;
                var deltaY = lhs.Height - rhs.Height;
                return deltaX * deltaX + deltaY * deltaY;
            }
        }

        /// <summary>
        /// Determines the expected size of the output <see cref="SKBitmap"/> from an input <see cref="SKBitmap"/> with size
        /// <paramref name="sourceSize"/>, where each character is <see cref="fontSize"/> pixels, and using a sampling factor
        /// of <paramref name="samplingFactor"/>.
        /// </summary>
        /// <param name="sourceSize">The size of the input <see cref="SKBitmap"/>.</param>
        /// <param name="fontSize">The space (in pixels) used by each character in the output <see cref="SKBitmap"/>.</param>
        /// <param name="samplingFactor">
        ///     The sampling factor to use on the input <see cref="SKBitmap"/>.
        ///     See docs for <see cref="TxBitmap"/> for the semantics of this value.
        /// </param>
        /// <returns>The expected size of the output <see cref="SKBitmap"/> of characters.</returns>
        public static SKSizeI ExpectedOutputSize(SKSizeI sourceSize, float fontSize, int samplingFactor)
        {
            var fontWidth = sourceSize.Width * fontSize;
            var fontHeight = sourceSize.Height * fontSize;
            return new SKSizeI(
                (int)Math.Ceiling(fontWidth / samplingFactor),
                (int)Math.Ceiling(fontHeight / samplingFactor));
        }

        /// <summary>
        /// Determines if <paramref name="samplingFactor"/> is a perfect sampling factor for an input <see cref="SKBitmap"/>
        /// of size <see cref="size"/>. See docs for <see cref="TxBitmap"/> for the definition of <i>perfect sampling factor</i>.
        /// </summary>
        /// <param name="size">The size of the input <see cref="SKBitmap"/>.</param>
        /// <param name="samplingFactor">
        ///     The sampling factor to use on the input <see cref="SKBitmap"/>.
        ///     See docs for <see cref="TxBitmap"/> for the semantics of this value.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="samplingFactor"/> is a perfect sampling factor for <paramref name="size"/>.
        ///     <c>false</c> otherwise.
        /// </returns>
        public static bool IsPerfectSamplingFactor(SKSizeI size, int samplingFactor) =>
            samplingFactor > 0 && (size.Width % samplingFactor == 0) && (size.Height % samplingFactor == 0);

        /// <summary>
        /// Returns every possible value that divides <paramref name="bitmap"/>'s width and height evenly.
        /// This value depends only on the bitmap size; pixel data does not matter.
        /// </summary>
        /// <param name="bitmap">The bitmap whose dimensions to check.</param>
        /// <returns>Every possible factor.</returns>
        /// <seealso cref="PerfectSamplingFactors(int,int)"/>
        public static IEnumerable<int> PerfectSamplingFactors(SKBitmap bitmap) =>
            PerfectSamplingFactors(bitmap.Width, bitmap.Height);

        /// <summary>
        /// Returns every possible value that divides <paramref name="size"/>'s width and height evenly.
        /// </summary>
        /// <param name="size">The size of the bitmap to check.</param>
        /// <returns>Every possible factor.</returns>
        /// <seealso cref="PerfectSamplingFactors(int,int)"/>
        public static IEnumerable<int> PerfectSamplingFactors(SKSizeI size) =>
            PerfectSamplingFactors(size.Width, size.Height);

        /// <summary>
        /// Returns every possible value that divides the width and height evenly.
        /// </summary>
        /// <param name="width">The width of bitmap to check.</param>
        /// <param name="height">The height of bitmap to check.</param>
        /// <returns>Every possible factor.</returns>
        /// <seealso cref="PerfectSamplingFactors(SKBitmap)"/>
        public static IEnumerable<int> PerfectSamplingFactors(int width, int height)
        {
            // The idea is to get every common factor between the width and the height
            Debug.Assert(width > 0);
            Debug.Assert(height > 0);

            // All common factors are <= the greatest common factor by definition.
            // So listing all factors of the gcd is sufficient.
            var gcd = Gcd(width, height);
            Debug.Assert(gcd >= 1);

            var checkMax = (int) Math.Sqrt(gcd);
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

            static int Gcd(int a, int b)
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

using System.Collections.Generic;
using System.Threading.Tasks;
using SkiaSharp;

namespace Texart.Api
{
    /// <summary>
    /// An <see cref="ITxTextBitmapGenerator"/> is used to generate <see cref="ITxTextBitmap"/>s given
    /// some configuration options.
    /// </summary>
    /// <seealso cref="ITxTextBitmap"/>
    /// <seealso cref="ITxTextBitmapRenderer"/>
    public interface ITxTextBitmapGenerator
    {
        // TODO: Move the docs for PixelSamplingRatio to the argument extraction API
//        /// <summary>
//        /// Gets the ratio of source resolution to generated resolution. That is, one
//        /// pixel in the generated text will come from sampling <c>Math.Pow(PixelSamplingRatio, 2)</c>
//        /// pixels.
//        /// For example, if <see cref="PixelSamplingRatio"/> is <c>2</c>, then <c>4</c>
//        /// pixels from the image will be used to generate <c>1</c> character.
//        /// Consequently, a value of <c>1</c> is loss-less.
//        /// Note that implementations are only required to support ratios that perfectly divide both the width
//        /// and height of the provided bitmap.
//        /// For the sake of maintaining aspect ratio, implementations must "chunk" images evenly on both X and
//        /// Y axes (using this many pixels).
//        /// This creates the unfortunately scenario in the case that the width and height are distinct prime
//        /// numbers (where the only valid sampling ratio is <c>1</c>). In that case, the image should be
//        /// resized or cropped before text generation.
//        /// </summary>
//        /// <see cref="GenerateAsync"/>
//        /// <see cref="TxBitmap.GetPerfectPixelRatios(SkiaSharp.SKBitmap)"/>
//        int PixelSamplingRatio { get; }

        /// <summary>
        /// Generates one or more <see cref="ITxTextBitmap"/>s from <paramref name="bitmaps"/>.
        /// </summary>
        /// <param name="bitmaps">The bitmaps to generate data from.</param>
        /// <returns>Async stream of generated <see cref="ITxTextBitmap"/>.</returns>
        IAsyncEnumerable<ITxTextBitmap> GenerateAsync(IAsyncEnumerable<SKBitmap> bitmaps);
    }
}

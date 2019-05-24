using System.Collections.Generic;
using System.Threading.Tasks;
using SkiaSharp;

namespace Texart.Api
{
    /// <summary>
    /// An <code>ITextBitmapGenerator</code> is used to generate <code>ITextBitmap</code>s given
    /// some configuration options.
    /// </summary>
    /// <see cref="ITextBitmap"/>
    /// <see cref="ITextBitmapRenderer"/>
    public interface ITextBitmapGenerator
    {
        /// <summary>
        /// Gets the available characters that will be used in the generated text.
        /// This listed shall be sorted from brightest characters to darkest characters.
        /// The definitions of dark and bright are implementation-defined.
        /// </summary>
        IList<char> Characters { get; }

        /// <summary>
        /// Gets the ratio of source resolution to generated resolution. That is, one
        /// pixel in the generated text will come from sampling <code>Math.Pow(PixelSamplingRatio, 2)</code>
        /// pixels.
        /// For example, if <code>PixelSamplingRatio</code> is <code>2</code>, then <code>4</code>
        /// pixels from the image will be used to generate <code>1</code> character.
        /// Consequently, a value of <code>1</code> is loss-less.
        /// Note that implementations are only required to support ratios that perfectly divide both the width
        /// and height of the provided bitmap.
        /// For the sake of maintaining aspect ratio, implementations must "chunk" images evenly on both X and
        /// Y axes (using this many pixels).
        /// This creates the unfortunately scenario in the case that the width and height are distinct prime
        /// numbers (where the only valid sampling ratio is <code>1</code>). In that case, the image should be
        /// resized or cropped before text generation.
        /// </summary>
        /// <see cref="GenerateAsync"/>
        /// <see cref="TxBitmap.GetPerfectPixelRatios(SkiaSharp.SKBitmap)"/>
        int PixelSamplingRatio { get; }

        /// <summary>
        /// Generates a <code>ITextBitmap</code> with the <code>PixelSamplingRatio</code> adjusted dimensions
        /// and using the available characters in <code>Characters</code>.
        /// </summary>
        /// <param name="bitmap">The bitmap to generate data from.</param>
        /// <returns></returns>
        /// <see cref="PixelSamplingRatio"/>
        /// <see cref="Characters"/>
        Task<ITextBitmap> GenerateAsync(SKBitmap bitmap);
    }
}

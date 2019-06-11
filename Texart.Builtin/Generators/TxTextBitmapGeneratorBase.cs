using System.Diagnostics;
using System.Collections.Generic;
using SkiaSharp;
using System;
using System.Threading.Tasks;
using Texart.Api;

namespace Texart.Builtin.Generators
{
    internal abstract class TxTextBitmapGeneratorBase : ITxTextBitmapGenerator
    {
        /// <summary>
        /// The constructor that should be called from derived types.
        /// </summary>
        /// <param name="characters">The character set to use.</param>
        /// <param name="pixelSamplingRatio">Ratio of pixel count (squared) to generated character count</param>
        protected TxTextBitmapGeneratorBase(IList<char> characters, int pixelSamplingRatio)
        {
            Characters = characters ?? throw new ArgumentNullException(nameof(characters));
            if (Characters.Count < 1)
            {
                throw new ArgumentException($"{nameof(characters)} must have at least 1 character.");
            }

            if (pixelSamplingRatio < 1)
            {
                throw new ArgumentException($"{nameof(pixelSamplingRatio)} must be at least 1.");
            }
            PixelSamplingRatio = pixelSamplingRatio;
        }

        /// <inheritdocs/>
        protected IList<char> Characters { get; set; }

        protected int PixelSamplingRatio { get; set; }

        /// <inheritdocs/>
        public async IAsyncEnumerable<ITxTextBitmap> GenerateAsync(IAsyncEnumerable<SKBitmap> bitmaps)
        {
            if (bitmaps is null) { throw new ArgumentNullException(nameof(bitmaps)); }

            await foreach (var bitmap in bitmaps)
            {
                if (bitmap.Width % PixelSamplingRatio != 0 || bitmap.Height % PixelSamplingRatio != 0)
                {
                    throw new ArgumentException($"{nameof(PixelSamplingRatio)} must evenly divide both Bitmap width and height.");
                }
                yield return await DoGenerateTextAsync(bitmap);
            }
        }

        /// <summary>
        /// The method that will perform the text generation after <paramref name="bitmap"/> has
        /// been checked for potential errors.
        /// </summary>
        /// <param name="bitmap">The image to generate text data from.</param>
        /// <returns>The generated text data.</returns>
        /// <see cref="ITxTextBitmapGenerator.GenerateAsync"/>
        protected abstract Task<ITxTextBitmap> DoGenerateTextAsync(SKBitmap bitmap);

        /// <summary>
        /// Gets the width for the given bitmap adjusted for the sampling ratio.
        /// </summary>
        /// <param name="bitmap">The bitmap to get width for.</param>
        /// <returns>The adjusted width.</returns>
        /// <see cref="PixelSamplingRatio"/>
        protected int WidthFor(SKBitmap bitmap)
        {
            Debug.Assert(bitmap.Width % PixelSamplingRatio == 0);
            return bitmap.Width / PixelSamplingRatio;
        }

        /// <summary>
        /// Gets the height for the given bitmap adjusted for the sampling ratio.
        /// </summary>
        /// <param name="bitmap">The bitmap to get height for.</param>
        /// <returns>The adjusted height.</returns>
        /// <see cref="PixelSamplingRatio"/>
        protected int HeightFor(SKBitmap bitmap)
        {
            Debug.Assert(bitmap.Height % PixelSamplingRatio == 0);
            return bitmap.Height / PixelSamplingRatio;
        }
    }
}

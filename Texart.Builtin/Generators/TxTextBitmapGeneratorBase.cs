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
        /// <param name="pixelSamplingRatio"><see cref="ITxTextBitmapGenerator.PixelSamplingRatio"/></param>
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

        /// <inheritdocs/>
        public int PixelSamplingRatio { get; protected set; }

        /// <inheritdocs/>
        public async Task<ITxTextBitmap> GenerateAsync(SKBitmap bitmap)
        {
            if (bitmap is null) { throw new ArgumentNullException(nameof(bitmap)); }
            if (bitmap.Width % this.PixelSamplingRatio != 0 || bitmap.Height % this.PixelSamplingRatio != 0)
            {
                throw new ArgumentException($"{nameof(this.PixelSamplingRatio)} must evenly divide both Bitmap width and height.");
            }
            return await this.DoGenerateTextAsync(bitmap);
        }

        /// <summary>
        /// The method that will perform the text generation after <paramref name="bitmap"/> has
        /// been checked for potential errors.
        /// </summary>
        /// <param name="bitmap">The image to generate text data from.</param>
        /// <returns>The generated text data.</returns>
        /// <see cref="ITxTextBitmapGenerator.GenerateAsync"/>
        public abstract Task<ITxTextBitmap> DoGenerateTextAsync(SKBitmap bitmap);

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

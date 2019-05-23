using System.Diagnostics;
using System.Collections.Generic;
using SkiaSharp;
using System;
using System.Threading.Tasks;
using Texart.Interface;

namespace Texart.Builtin.Generators
{
    public abstract class TextGeneratorBase : ITextGenerator
    {
        protected TextGeneratorBase(IList<char> characters, int pixelSamplingRatio)
        {
            this.Characters = characters ?? throw new ArgumentNullException(nameof(characters));
            if (Characters.Count < 1)
            {
                throw new ArgumentException($"{nameof(characters)} must have at least 1 character.");
            }

            if (pixelSamplingRatio < 1)
            {
                throw new ArgumentException($"{nameof(pixelSamplingRatio)} must be at least 1.");
            }
            this.PixelSamplingRatio = pixelSamplingRatio;
        }

        /// <inheritdocs/>
        public IList<char> Characters { get; protected set; }

        /// <inheritdocs/>
        public int PixelSamplingRatio { get; protected set; }

        /// <inheritdocs/>
        public async Task<ITextData> GenerateTextAsync(Bitmap bitmap)
        {
            if (bitmap == null) { throw new ArgumentNullException(nameof(bitmap)); }
            if (bitmap.SkiaBitmap == null) { throw new ArgumentNullException(nameof(bitmap.SkiaBitmap)); }
            if (bitmap.Width % this.PixelSamplingRatio != 0 || bitmap.Height % this.PixelSamplingRatio != 0)
            {
                throw new ArgumentException($"{nameof(this.PixelSamplingRatio)} must evenly divide both Bitmap width and height.");
            }
            return await this.DoGenerateTextAsync(bitmap.SkiaBitmap);
        }

        /// <summary>
        /// The method that will perform the text generation after <code>bitmap</code> has
        /// been checked for potential errors.
        /// </summary>
        /// <param name="bitmap">The image to generate text data from.</param>
        /// <returns>The generated text data.</returns>
        /// <see cref="ITextGenerator.GenerateTextAsync(Bitmap)"/>
        public abstract Task<ITextData> DoGenerateTextAsync(SKBitmap bitmap);

        /// <summary>
        /// Gets the width for the given bitmap adjusted for the sampling ratio.
        /// </summary>
        /// <param name="bitmap">The bitmap to get width for.</param>
        /// <returns>The adjusted width.</returns>
        /// <see cref="PixelSamplingRatio"/>
        protected int WidthFor(SKBitmap bitmap)
        {
            Debug.Assert(bitmap.Width % this.PixelSamplingRatio == 0);
            return bitmap.Width / this.PixelSamplingRatio;
        }

        /// <summary>
        /// Gets the height for the given bitmap adjusted for the sampling ratio.
        /// </summary>
        /// <param name="bitmap">The bitmap to get height for.</param>
        /// <returns>The adjusted height.</returns>
        /// <see cref="PixelSamplingRatio"/>
        protected int HeightFor(SKBitmap bitmap)
        {
            Debug.Assert(bitmap.Height % this.PixelSamplingRatio == 0);
            return bitmap.Height / this.PixelSamplingRatio;
        }
    }
}

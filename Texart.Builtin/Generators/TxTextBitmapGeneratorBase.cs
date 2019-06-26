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
        /// <param name="samplingFactor">Ratio of pixel count (squared) to generated character count</param>
        protected TxTextBitmapGeneratorBase(IList<char> characters, int samplingFactor)
        {
            Characters = characters ?? throw new ArgumentNullException(nameof(characters));
            if (Characters.Count < 1)
            {
                throw new ArgumentException($"{nameof(characters)} must have at least 1 character.");
            }

            if (samplingFactor < 1)
            {
                throw new ArgumentException($"{nameof(samplingFactor)} must be at least 1.");
            }
            SamplingFactor = samplingFactor;
        }

        /// <summary>
        /// The characters to use in the output <see cref="ITxTextBitmap"/>.
        /// </summary>
        protected IList<char> Characters { get; set; }

        protected int SamplingFactor { get; set; }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ITxTextBitmap> GenerateAsync(IAsyncEnumerable<SKBitmap> bitmaps)
        {
            if (bitmaps is null) { throw new ArgumentNullException(nameof(bitmaps)); }

            await foreach (var bitmap in bitmaps)
            {
                if (bitmap.Width % SamplingFactor != 0 || bitmap.Height % SamplingFactor != 0)
                {
                    throw new ArgumentException($"{nameof(SamplingFactor)} must evenly divide both Bitmap width and height.");
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
        /// <see cref="SamplingFactor"/>
        protected int WidthFor(SKBitmap bitmap)
        {
            Debug.Assert(bitmap.Width % SamplingFactor == 0);
            return bitmap.Width / SamplingFactor;
        }

        /// <summary>
        /// Gets the height for the given bitmap adjusted for the sampling ratio.
        /// </summary>
        /// <param name="bitmap">The bitmap to get height for.</param>
        /// <returns>The adjusted height.</returns>
        /// <see cref="SamplingFactor"/>
        protected int HeightFor(SKBitmap bitmap)
        {
            Debug.Assert(bitmap.Height % SamplingFactor == 0);
            return bitmap.Height / SamplingFactor;
        }
    }
}

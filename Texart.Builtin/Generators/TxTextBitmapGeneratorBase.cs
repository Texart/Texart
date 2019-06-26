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
        protected TxTextBitmapGeneratorBase(IList<char> characters)
        {
            Characters = characters ?? throw new ArgumentNullException(nameof(characters));
            if (Characters.Count < 1)
            {
                throw new ArgumentException($"{nameof(characters)} must have at least 1 character.");
            }
        }

        /// <summary>
        /// The characters to use in the output <see cref="ITxTextBitmap"/>.
        /// </summary>
        protected IList<char> Characters { get; set; }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ITxTextBitmap> GenerateAsync(IAsyncEnumerable<SKBitmap> bitmaps)
        {
            if (bitmaps is null) { throw new ArgumentNullException(nameof(bitmaps)); }

            await foreach (var bitmap in bitmaps)
            {
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
    }
}

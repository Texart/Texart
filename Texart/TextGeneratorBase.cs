using System.Diagnostics;
using System.Collections.Generic;
using SkiaSharp;
using System;

namespace Texart
{
    public abstract class TextGeneratorBase : ITextGenerator
    {
        public TextGeneratorBase(SKBitmap bitmap, IList<char> characters, int pixelSamplingRatio)
        {
            if (bitmap == null) { throw new ArgumentNullException(nameof(bitmap)); }
            Bitmap = bitmap;

            if (characters == null) { throw new ArgumentNullException(nameof(characters)); }
            Characters = characters;
            if (Characters.Count < 1)
            {
                throw new ArgumentException($"{nameof(characters)} must have at least 1 character.");
            }

            if (pixelSamplingRatio < 1)
            {
                throw new ArgumentException($"{nameof(pixelSamplingRatio)} must be at least 1.");
            }
            if (!(bitmap.Width % pixelSamplingRatio == 0) || !(bitmap.Height % pixelSamplingRatio == 0))
            {
                throw new ArgumentException($"{nameof(pixelSamplingRatio)} must evenly divide both Bitmap width and height.");
            }
            PixelSamplingRatio = pixelSamplingRatio;
        }

        /// <inheritdocs/>
        public int Width { get { return Bitmap.Width / PixelSamplingRatio; } }

        /// <inheritdocs/>
        public int Height { get { return Bitmap.Height / PixelSamplingRatio; } }

        /// <inheritdocs/>
        public abstract IList<char> Characters { get; protected set; }

        /// <inheritdocs/>
        public SKBitmap Bitmap { get; protected set; }

        /// <inheritdocs/>
        public int PixelSamplingRatio { get; protected set; }

        /// <inheritdocs/>
        public abstract ITextData GenerateText();
    }
}

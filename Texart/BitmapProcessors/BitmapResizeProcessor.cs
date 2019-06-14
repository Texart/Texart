using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Texart.BitmapProcessors
{
    /// <summary>
    /// An <see cref="IBitmapProcessor{T}"/> that resizes a given bitmap.
    /// </summary>
    public sealed class BitmapResizeProcessor : IBitmapProcessor<SKBitmap>
    {
        /// <summary>
        /// The size of the processed (output) bitmap. 
        /// </summary>
        public SKSizeI TargetSize { get; }

        /// <summary>
        /// The resize filter quality.
        /// </summary>
        public SKFilterQuality FilterQuality { get; }

        /// <summary>
        /// Creates a <see cref="BitmapResizeProcessor"/> with the provided target size.
        /// </summary>
        /// <param name="targetSize">The size of the processed (output) bitmap.</param>
        /// <param name="filterQuality">The resize filter quality.</param>
        /// <exception cref="ArgumentException">
        ///     If <c>targetSize.Width</c> or <c>targetSize.Height</c> are negative.
        /// </exception>
        public BitmapResizeProcessor(SKSizeI targetSize, SKFilterQuality filterQuality)
        {
            if (targetSize.Width < 0)
            {
                throw new ArgumentException($"{nameof(targetSize.Width)} must be non-negative.");
            }
            if (targetSize.Height < 0)
            {
                throw new ArgumentException($"{nameof(targetSize.Height)} must be non-negative.");
            }
            TargetSize = targetSize;
            FilterQuality = filterQuality;
        }

        /// <inheritdoc cref="IBitmapProcessor{T}.Process"/>
        public async IAsyncEnumerable<SKBitmap> Process(IAsyncEnumerable<SKBitmap> bitmaps)
        {
            await foreach (var bitmap in bitmaps)
            {
                var resizedImageInfo = bitmap.Info.WithSize(TargetSize.Width, TargetSize.Height);
                yield return bitmap.Resize(resizedImageInfo, FilterQuality);
            }
        }
    }
}
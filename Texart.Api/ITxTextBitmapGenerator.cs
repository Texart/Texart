using System.Collections.Generic;
using SkiaSharp;

#nullable enable

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
        /// <summary>
        /// Generates one or more <see cref="ITxTextBitmap"/>s from <paramref name="bitmaps"/>.
        /// </summary>
        /// <param name="bitmaps">The bitmaps to generate data from.</param>
        /// <returns>Async stream of generated <see cref="ITxTextBitmap"/>.</returns>
        IAsyncEnumerable<ITxTextBitmap> GenerateAsync(IAsyncEnumerable<SKBitmap> bitmaps);
    }
}

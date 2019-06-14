using System.Collections.Generic;
using System.Threading.Tasks;
using SkiaSharp;

namespace Texart
{
    /// <summary>
    /// An <see cref="IBitmapProcessor{T}"/> processes <see cref="SKBitmap"/> into an output of type <typeparamref name="T"/>.
    /// A processor implementation should be able to handle an async stream of <see cref="SKBitmap"/>.
    /// </summary>
    /// <typeparam name="T">The processed (output) type.</typeparam>
    public interface IBitmapProcessor<out T>
    {
        /// <summary>
        /// Processes a stream of incoming <see cref="SKBitmap"/> to produce a stream of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="bitmaps">The bitmaps to process.</param>
        /// <returns>Async stream of processed <typeparamref name="T"/>.</returns>
        IAsyncEnumerable<T> Process(IAsyncEnumerable<SKBitmap> bitmaps);
    }
}
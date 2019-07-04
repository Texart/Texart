using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

#nullable enable

namespace Texart.Api
{
    /// <summary>
    /// An <see cref="ITxTextBitmapRenderer" /> writes some text data into an output stream
    /// in an implementation-defined format.
    /// </summary>
    /// <seealso cref="ITxTextBitmap"/>
    /// <seealso cref="ITxTextBitmapGenerator"/>
    public interface ITxTextBitmapRenderer
    {
        /// <summary>
        /// Write some text data to an output stream.
        /// </summary>
        /// <param name="textBitmaps">The source text data.</param>
        /// <param name="outputStream">The stream to write to.</param>
        Task RenderAsync(IAsyncEnumerable<ITxTextBitmap> textBitmaps, Stream outputStream);
    }
}
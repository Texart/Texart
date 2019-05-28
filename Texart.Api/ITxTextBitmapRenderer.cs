using System.IO;
using System.Threading.Tasks;

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
        /// <param name="txTextBitmap">The source text data</param>
        /// <param name="outputStream">The stream to write to</param>
        Task RenderAsync(ITxTextBitmap txTextBitmap, Stream outputStream);
    }
}

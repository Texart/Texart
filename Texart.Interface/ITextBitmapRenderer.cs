using System.IO;
using System.Threading.Tasks;

namespace Texart.Interface
{
    /// <summary>
    /// An <code>ITextBitmapRenderer</code> writes some text data into an output stream
    /// in an implementation-defined format.
    /// </summary>
    /// <see cref="ITextBitmap"/>
    /// <see cref="ITextBitmapGenerator"/>
    public interface ITextBitmapRenderer
    {
        /// <summary>
        /// Write some text data to an output stream.
        /// </summary>
        /// <param name="textBitmap">The source text data</param>
        /// <param name="outputStream">The stream to write to</param>
        Task RenderAsync(ITextBitmap textBitmap, Stream outputStream);
    }
}

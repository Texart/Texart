using System.IO;
using System.Threading.Tasks;

namespace Texart.Interface
{
    /// <summary>
    /// An <code>ITextRenderer</code> writes some text data into an output stream
    /// in an implementation-defined format.
    /// </summary>
    /// <see cref="ITextData"/>
    /// <see cref="ITextGenerator"/>
    public interface ITextRenderer
    {
        /// <summary>
        /// Write some text data to an output stream.
        /// </summary>
        /// <param name="textData">The source text data</param>
        /// <param name="outputStream">The stream to write to</param>
        Task RenderAsync(ITextData textData, Stream outputStream);
    }
}

using System.IO;
using System.Threading.Tasks;

namespace Texart
{
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

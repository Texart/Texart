using System.IO;
using System.Threading.Tasks;

namespace Texart
{
    public interface ITextDataRenderer
    {
        /// <summary>
        /// Write some text data to an output stream.
        /// </summary>
        /// <param name="textData">The source text data</param>
        /// <param name="outputStream">The stream to write to</param>
        Task Render(ITextData textData, Stream outputStream);
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Texart.Interface;

namespace Texart.Builtin
{
    /// <summary>
    /// A renderer that simply writes the text data to the output
    /// as a string.
    /// </summary>
    public sealed class StringRenderer : ITextRenderer
    {
        public Encoding Encoding { get; }

        /// <inheritdocs/>
        public Task RenderAsync(ITextData textData, Stream outputStream)
        {
            Debug.Assert(textData != null);
            Debug.Assert(outputStream != null);

            outputStream = (outputStream is BufferedStream) ?
                outputStream : new BufferedStream(outputStream);

            using (TextWriter writer = new StreamWriter(outputStream, Encoding))
            {
                for (var y = 0; y < textData.Height; ++y)
                {
                    for (var x = 0; x < textData.Width; ++x)
                    {
                        writer.Write((char)textData[x, y]);
                    }
                    writer.Write(writer.NewLine);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a <code>StringRenderer</code> with <code>DefaultEncoding</code>.
        /// </summary>
        /// <see cref="PlatformHelpers.DefaultEncoding"/>
        /// <see cref="StringRenderer(Encoding)"/>
        /// <see cref="System.Text.Encoding"/>
        public StringRenderer()
            : this(PlatformHelpers.DefaultEncoding)
        {
        }

        /// <summary>
        /// Creates a <code>StringRenderer</code> with the given encoding.
        /// </summary>
        /// <param name="encoding">The encoding to write with</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <code>encoding</code>
        ///     is <code>null</code>.
        /// </exception>
        public StringRenderer(Encoding encoding)
        {
            if (encoding == null) { throw new ArgumentNullException(nameof(encoding)); }
            Encoding = encoding;
        }
    }
}

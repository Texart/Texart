using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Texart.Builtin.Internal;
using Texart.Api;

namespace Texart.Builtin.Renderers
{
    /// <summary>
    /// A renderer that simply writes the text data to the output
    /// as a string.
    /// </summary>
    internal sealed class StringBitmapRenderer : ITxTextBitmapRenderer
    {
        /// <summary>
        /// The encoding to output as.
        /// </summary>
        public Encoding Encoding { get; }

        /// <inheritdocs/>
        public Task RenderAsync(ITxTextBitmap txTextBitmap, Stream outputStream)
        {
            Debug.Assert(txTextBitmap != null);
            Debug.Assert(outputStream != null);

            outputStream = (outputStream is BufferedStream) ?
                outputStream : new BufferedStream(outputStream);

            using (TextWriter writer = new StreamWriter(outputStream, Encoding))
            {
                for (var y = 0; y < txTextBitmap.Height; ++y)
                {
                    for (var x = 0; x < txTextBitmap.Width; ++x)
                    {
                        writer.Write((char)txTextBitmap.CharAt(x, y));
                    }
                    writer.Write(writer.NewLine);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a <see cref="StringBitmapRenderer"/> with <see cref="PlatformHelpers.DefaultEncoding"/>.
        /// </summary>
        /// <seealso cref="PlatformHelpers.DefaultEncoding"/>
        /// <seealso cref="StringBitmapRenderer(System.Text.Encoding)"/>
        /// <seealso cref="System.Text.Encoding"/>
        public StringBitmapRenderer()
            : this(PlatformHelpers.DefaultEncoding)
        {
        }

        /// <summary>
        /// Creates a <see cref="StringBitmapRenderer"/> with the given encoding.
        /// </summary>
        /// <param name="encoding">The encoding to write with</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="encoding"/> is <c>null</c>.
        /// </exception>
        public StringBitmapRenderer(Encoding encoding)
        {
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        /// <summary>
        /// Factory function for <see cref="Plugin"/>.
        /// </summary>
        /// <param name="args">Input arguments.</param>
        /// <returns>Constructed instance.</returns>
        public static StringBitmapRenderer Create(TxArguments args)
        {
            // TODO: use args
            return new StringBitmapRenderer();
        }
    }
}

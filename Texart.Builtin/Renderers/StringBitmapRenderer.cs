using System;
using System.Collections.Generic;
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
        private Encoding Encoding { get; }

        /// <inheritdocs/>
        public async Task RenderAsync(IAsyncEnumerable<ITxTextBitmap> textBitmaps, Stream outputStream)
        {
            Debug.Assert(textBitmaps != null);
            Debug.Assert(outputStream != null);

            outputStream = outputStream is BufferedStream
                ? outputStream
                : new BufferedStream(outputStream);

            await foreach (var textBitmap in textBitmaps)
            {
                await using TextWriter writer = new StreamWriter(outputStream, Encoding);
                for (var y = 0; y < textBitmap.Height; ++y)
                {
                    for (var x = 0; x < textBitmap.Width; ++x)
                    {
                        writer.Write(textBitmap.CharAt(x, y));
                    }
                    writer.Write(writer.NewLine);
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="StringBitmapRenderer"/> with <see cref="PlatformHelpers.DefaultEncoding"/>.
        /// </summary>
        /// <seealso cref="PlatformHelpers.DefaultEncoding"/>
        /// <seealso cref="StringBitmapRenderer(System.Text.Encoding)"/>
        /// <seealso cref="System.Text.Encoding"/>
        private StringBitmapRenderer()
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
        private StringBitmapRenderer(Encoding encoding)
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

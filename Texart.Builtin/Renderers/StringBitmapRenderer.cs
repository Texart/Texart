using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Texart.Builtin.Internal;
using Texart.Api;

namespace Texart.Builtin.Renderers
{
    /// <summary>
    /// A renderer that simply writes the text data to the output
    /// as a string.
    /// </summary>
    public sealed class StringBitmapRenderer : ITextBitmapRenderer
    {
        public Encoding Encoding { get; }

        /// <inheritdocs/>
        public Task RenderAsync(ITextBitmap textBitmap, Stream outputStream)
        {
            Debug.Assert(textBitmap != null);
            Debug.Assert(outputStream != null);

            outputStream = (outputStream is BufferedStream) ?
                outputStream : new BufferedStream(outputStream);

            using (TextWriter writer = new StreamWriter(outputStream, Encoding))
            {
                for (var y = 0; y < textBitmap.Height; ++y)
                {
                    for (var x = 0; x < textBitmap.Width; ++x)
                    {
                        writer.Write((char)textBitmap[x, y]);
                    }
                    writer.Write(writer.NewLine);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a <code>StringBitmapRenderer</code> with <code>DefaultEncoding</code>.
        /// </summary>
        /// <see cref="PlatformHelpers.DefaultEncoding"/>
        /// <see cref="StringBitmapRenderer(System.Text.Encoding)"/>
        /// <see cref="System.Text.Encoding"/>
        public StringBitmapRenderer()
            : this(PlatformHelpers.DefaultEncoding)
        {
        }

        /// <summary>
        /// Creates a <code>StringBitmapRenderer</code> with the given encoding.
        /// </summary>
        /// <param name="encoding">The encoding to write with</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <code>encoding</code>
        ///     is <code>null</code>.
        /// </exception>
        public StringBitmapRenderer(Encoding encoding)
        {
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public static StringBitmapRenderer Create(Lazy<JToken> json)
        {
            // TODO: use json
            return new StringBitmapRenderer();
        }
    }
}

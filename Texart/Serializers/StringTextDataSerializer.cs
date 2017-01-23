using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Texart.Serializers
{
    /// <summary>
    /// A serializer that simply writes the text data to the output
    /// as a string.
    /// </summary>
    public class StringTextDataSerializer : ITextDataSerializer
    {
        public Encoding Encoding { get; }

        /// <inheritdocs/>
        public void Write(ITextData textData, Stream outputStream)
        {
            Debug.Assert(textData != null);
            Debug.Assert(outputStream != null);

            outputStream = (outputStream is BufferedStream) ?
                outputStream : new BufferedStream(outputStream);

            using (TextWriter writer = new StreamWriter(outputStream, Encoding.UTF8))
            {
                for (var y = 0; y < textData.Height; ++y)
                {
                    for (var x = 0; x < textData.Width; ++x)
                    {
                        writer.Write((char)textData[x, y]);
                    }
                    writer.Write('\n');
                }
            }
        }

        /// <summary>
        /// Creates a <code>StringTextDataSerializer</code> with <code>Encoding</code> of UTF8.
        /// </summary>
        /// <see cref="StringTextDataSerializer(Encoding)"/>
        /// <see cref="System.Text.Encoding"/>
        public StringTextDataSerializer()
            : this(Encoding.UTF8)
        {
        }

        /// <summary>
        /// Creates a <code>StringTextDataSerializer</code> with the given encoding.
        /// </summary>
        /// <param name="encoding">The encoding to write with</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <code>encoding</code>
        ///     is <code>null</code>.
        /// </exception>
        public StringTextDataSerializer(Encoding encoding)
        {
            if (encoding == null) { throw new ArgumentNullException(nameof(encoding)); }
            Encoding = encoding;
        }
    }
}

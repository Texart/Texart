using System.Diagnostics;
using System.IO;
using System.Text;

namespace Texart.Serializers
{
    public class UTF8TextDataSerializer : ITextDataSerializer
    {
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
    }
}

using System.IO;
using System.Threading.Tasks;

namespace Texart.ScriptInterface
{
    public static partial class Tx
    {
        public static async Task Render(ITextRenderer renderer, ITextData textData, Stream output)
        {
            await renderer.RenderAsync(textData, output);
        }
    }
}

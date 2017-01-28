using System.IO;
using System.Threading.Tasks;

namespace Texart.ScriptInterface
{
    public static partial class Tx
    {
        public static async Task<ITextData> Generate(ITextGenerator renderer, Bitmap bitmap)
        {
            return await renderer.GenerateTextAsync(bitmap: bitmap.SkiaBitmap);
        }
    }
}

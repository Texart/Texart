using System.IO;
using System.Threading.Tasks;
using SkiaSharp;
using Texart.Api;

namespace Texart
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            using (var output = File.OpenWrite("../../../../meme.gen.png"))
            {
                var bitmap = TxContract.NonNull(SKBitmap.Decode("../../../../meme.jpg"));
                IPlugin builtinPlugin = new Builtin.Plugin();
                
                var textBitmapGenerator = builtinPlugin.LookupGenerator("BrightnessBasedBitmapGenerator")(TxArguments.Empty);
                var textBitmap = await textBitmapGenerator.GenerateAsync(bitmap);
                var textBitmapRenderer = builtinPlugin.LookupRenderer("FontBitmapRenderer")(TxArguments.Empty);
                await textBitmapRenderer.RenderAsync(textBitmap, output);
            }
        }
    }
}
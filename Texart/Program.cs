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
                var bitmap = SKBitmap.Decode("../../../../meme.jpg");
                IPlugin builtinPlugin = new Builtin.Plugin();

                // TODO: use JSON stream instead of null
                var textBitmapGenerator = builtinPlugin.LookupGenerator("BrightnessBasedBitmapGenerator")(null);
                var textBitmap = await textBitmapGenerator.GenerateAsync(bitmap);
                var textBitmapRenderer = builtinPlugin.LookupRenderer("FontBitmapRenderer")(null);
                await textBitmapRenderer.RenderAsync(textBitmap, output);
            }
        }
    }
}
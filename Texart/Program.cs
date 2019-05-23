using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Texart.ScriptInterface;
using Texart.Builtin.Generators;
using Texart.Builtin.Renderers;
using Texart.Interface;
using Texart.Plugins;

namespace Texart
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var output = File.OpenWrite("../../../../meme.gen.png"))
            {
                var bitmap = Bitmap.FromFile("../../../../meme.jpg");
                IPlugin builtinPlugin = new Builtin.Plugin();

                // TODO: use JSON stream instead of null
                var textBitmapGenerator = builtinPlugin.LookupGenerator("BrightnessBasedBitmapGenerator")(null);
                var textBitmap = await textBitmapGenerator.GenerateAsync(bitmap);
                var textBitmapRenderer = builtinPlugin.LookupRenderer("FontBitmapRenderer")(null);
                await textBitmapRenderer.RenderAsync(textBitmap, output);

                //Bitmap bitmap = Bitmap.FromFile("../../../../meme.jpg");
                //int scale = Tx.GetPerfectPixelRatios(bitmap).OrderBy(val => val).ElementAt(0);
                //ITextBitmapGenerator textBitmapGenerator = new BrightnessBasedBitmapGenerator(
                //    characters: Tx.CharacterSets.Basic,
                //    pixelSamplingRatio: scale
                //);
                //ITextBitmap textBitmap = await textBitmapGenerator.GenerateAsync(bitmap);
                //Font font = Font.FromTypeface(Typeface.FromName("Consolas"));
                //ITextBitmapRenderer textBitmapRenderer = new FontBitmapRenderer(font);
                //await textBitmapRenderer.RenderAsync(textBitmap, output);
            }
        }
    }
}
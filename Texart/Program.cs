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
                var textGenerator = builtinPlugin.LookupGenerator("BrightnessBasedGenerator")(null);
                var textData = await textGenerator.GenerateTextAsync(bitmap);
                var textRenderer = builtinPlugin.LookupRenderer("FontRenderer")(null);
                await textRenderer.RenderAsync(textData, output);

                //Bitmap bitmap = Bitmap.FromFile("../../../../meme.jpg");
                //int scale = Tx.GetPerfectPixelRatios(bitmap).OrderBy(val => val).ElementAt(0);
                //ITextGenerator textGenerator = new BrightnessBasedGenerator(
                //    characters: Tx.CharacterSets.Basic,
                //    pixelSamplingRatio: scale
                //);
                //ITextData textData = await textGenerator.GenerateTextAsync(bitmap);
                //Font font = Font.FromTypeface(Typeface.FromName("Consolas"));
                //ITextRenderer textRenderer = new FontRenderer(font);
                //await textRenderer.RenderAsync(textData, output);
            }
        }
    }
}
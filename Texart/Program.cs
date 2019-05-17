using System;
using SkiaSharp;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Texart.ScriptInterface;
using Texart.Builtin;
using Texart.Interface;

namespace Texart
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var output = File.OpenWrite("../../../../meme.gen.png"))
            {
                Bitmap bitmap = Bitmap.FromFile("../../../../meme.jpg");
                int scale = Tx.GetPerfectPixelRatios(bitmap).OrderBy(val => val).ElementAt(0);
                ITextGenerator textGenerator = new BrightnessBasedGenerator(
                    characters: Tx.CharacterSets.Basic,
                    pixelSamplingRatio: scale
                );
                ITextData textData = await textGenerator.GenerateTextAsync(bitmap);
                Font font = Font.FromTypeface(Typeface.FromName("Consolas"));
                ITextRenderer textRenderer = new FontRenderer(font);
                await textRenderer.RenderAsync(textData, output);
            }
        }
    }
}
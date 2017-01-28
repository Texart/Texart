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
        static async Task MainAsync(string[] args)
        {
            using (var output = File.OpenWrite("../../../../mona1.gen.png"))
            {
                Bitmap bitmap = Bitmap.FromFile("../../../../mona1.png");
                int scale = Tx.GetPerfectPixelRatios(bitmap).OrderBy(val => val).ElementAt(1);
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

        static void Main(string[] args)
        {
            var mainTask = Task.Run(async () =>
            {
                await MainAsync(args);
            });
            mainTask.GetAwaiter().GetResult();
        }
    }
}

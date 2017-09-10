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
            using (var output = File.OpenWrite("../../../../mona.gen.png"))
            {
                Bitmap bitmap = Bitmap.FromFile("../../../../mona.png");
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

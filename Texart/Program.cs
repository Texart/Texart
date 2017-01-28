using System;
using SkiaSharp;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Texart.ScriptInterface;

namespace Texart
{
    class Program
    {
        static async Task MainAsync(string[] args)
        {
            using (var output = File.OpenWrite("../../../../mona1.gen.png"))
            {
                Bitmap bitmap = Bitmap.FromFile("../../../../mona1.png");
                int scale = Tx.GetPerfectPixelRatios(bitmap).OrderBy(val => val).First();
                ITextGenerator textGenerator = Tx.CreateBrightnessBasedGenerator(
                    characterSet: CharacterSets.Basic,
                    pixelRatio: scale
                );
                ITextData textData = await Tx.Generate(textGenerator, bitmap);
                Font font = Font.FromTypeface(Typeface.FromName("Consolas"));
                ITextRenderer textRenderer = Tx.CreateFontRenderer(font);
                await Tx.Render(textRenderer, textData, output);
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

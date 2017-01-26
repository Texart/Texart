using System;
using SkiaSharp;
using System.IO;
using System.Threading.Tasks;

namespace Texart
{
    class Program
    {

        static void Main(string[] args)
        {
            var mainTask = Task.Run(async () =>
            {
                await MainAsync(args);
            });
            mainTask.GetAwaiter().GetResult();
        }


        static async Task MainAsync(string[] args)
        {
			using (SKStream stream = new SKManagedStream(File.OpenRead("../../../../mona.png")))
            using (FileStream output = File.OpenWrite("../../../../mona.gen.png"))
            {
                SKBitmap bitmap = SKBitmap.Decode(stream);
                const int scale = 2;
                ITextGenerator textGenerator = new Builtin.Generators.BrightnessBasedGenerator(
                    bitmap: bitmap,
                    characters: new[] {
                        ' ', ' ', ' ', ' ',
                        '.', '.',
                        ',', ',',
                        '-', '-', '-',
                        '~', '~', '~',
                        ':', ':',
                        ';', ';', ';',
                        '!', '!', '!',
                        '*', '*', '*',
                        '=', '=', '=', '=', '=',
                        '$', '$', '$', '$', '$', '$', '$',
                        '#', '#', '#', '#', '#', '#', '#', '#', '#', '#',
                        '&', '&', '&', '&', '&', '&', '&', '&', '&', '&',
                        '@', '@', '@', '@', '@', '@', '@', '@', '@', '@', '@', '@'
                    },
                    pixelSamplingRatio: scale
                );
                ITextData textData = await textGenerator.GenerateText();
                ITextDataRenderer textDataRenderer = new Builtin.Renderers.StringTextDataRenderer();
                var typeface = SKTypeface.FromFamilyName("Consolas", SKTypefaceStyle.Bold);
                ITextDataRenderer imageRenderer = new Builtin.Renderers.FontRasterizedTextDataRenderer(typeface);
                // await textDataRenderer.Render(textData, Console.OpenStandardOutput());
                await imageRenderer.Render(textData, output);
            }
        }
    }
}

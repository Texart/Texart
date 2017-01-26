using System;
using SkiaSharp;
using System.IO;
using System.Threading.Tasks;

namespace Texart
{
    class Program
    {
        static async Task MainAsync(string[] args)
        {
			using (SKStream stream = new SKManagedStream(File.OpenRead("../../../../mona1.png")))
            using (FileStream output = File.OpenWrite("../../../../mona1.gen.png"))
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
                ITextData textData = await textGenerator.GenerateTextAsync();
                ITextDataRenderer textDataRenderer = new Builtin.Renderers.StringTextDataRenderer();
                var typeface = SKTypeface.FromFamilyName("Consolas", SKTypefaceStyle.Bold);
                ITextDataRenderer imageRenderer = new Builtin.Renderers.FontRasterizedTextDataRenderer(typeface);
                // await textDataRenderer.RenderAsync(textData, Console.OpenStandardOutput());
                await imageRenderer.RenderAsync(textData, output);
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

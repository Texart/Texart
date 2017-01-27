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
                ITextData textData = await textGenerator.GenerateTextAsync(bitmap);
                ITextRenderer textDataRenderer = new Builtin.Renderers.StringRenderer();
                var typeface = SKTypeface.FromFamilyName("Consolas", SKTypefaceStyle.Bold);
                ITextRenderer imageRenderer = new Builtin.Renderers.FontRenderer(typeface, 12f, 8);
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

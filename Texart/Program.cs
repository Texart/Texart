using System;
using SkiaSharp;
using System.IO;

namespace Texart
{
    class Program
    {
        static void Main(string[] args)
        {
			using (SKStream stream = new SKManagedStream(File.OpenRead("../../../../mona.png")))
            using (FileStream output = File.OpenWrite("../../../../mona.gen.png"))
            {
                SKBitmap bitmap = SKBitmap.Decode(stream);
                // bitmap.LockPixels();
                const int scale = 2;
                ITextGenerator textGenerator = new Generators.BrightnessBasedGenerator(
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
                ITextData textData = textGenerator.GenerateText();
                ITextDataRenderer textDataRenderer = new Renderers.StringTextDataRenderer();
                var typeface = SKTypeface.FromFamilyName("Consolas", SKTypefaceStyle.Bold);
                ITextDataRenderer imageRenderer = new Renderers.FontRasterizedTextDataRenderer(typeface);
                // textDataRenderer.Render(textData, Console.OpenStandardOutput());
                imageRenderer.Render(textData, output);
            }
        }
    }
}

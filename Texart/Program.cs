using System;
using SkiaSharp;
using System.IO;

namespace Texart
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SKStream stream = new SKFileStream("C:/_img/bw1.png"))
            using (FileStream output = File.OpenWrite("C:/_img/bw2.png"))
            {
                SKBitmap bitmap = SKBitmap.Decode(stream);
                // bitmap.LockPixels();
                const int scale = 16;//64;
                ITextGenerator textGenerator = new Generators.BrightnessBasedGenerator(
                    bitmap: bitmap,
                    characters: new[] { ' ', '.', ',', '-', '~', ':', ';', '!', '*', '=', '$', '#', '@' },
                    pixelSamplingRatio: scale
                );
                ITextData textData = textGenerator.GenerateText();
                ITextDataSerializer textDataSerializer = new Serializers.UTF8TextDataSerializer();
                textDataSerializer.Write(textData, Console.OpenStandardOutput());
                /*
                using (var outBitmap = new SKBitmap(bitmap.Width, bitmap.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul))
                using (var canvas = new SKCanvas(outBitmap))
                using (var paint = new SKPaint())
                {
                    canvas.DrawBitmap(bitmap, SKRect.Create(bitmap.Width, bitmap.Height), paint);

                    SKImage.FromBitmap(outBitmap).Encode().SaveTo(output);
                }
                */
                // bitmap.UnlockPixels();
            }
        }
    }
}

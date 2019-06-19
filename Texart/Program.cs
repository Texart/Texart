using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SkiaSharp;
using Texart.Api;
using Texart.BitmapProcessors;

namespace Texart
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            CommandLineArgs.Parse(args);

            await using var output = File.OpenWrite("../../../../meme.gen.png");
            var bitmap = TxContract.NonNull(SKBitmap.Decode("../../../../meme.jpg"));
            var bitmaps = OneAsync(bitmap);

            IBitmapProcessor<SKBitmap> resizeProcessor = new BitmapResizeProcessor(
                new SKSizeI(1000, 1000), SKFilterQuality.High);

            ITxPlugin builtinPlugin = new Builtin.Plugin();

            var textBitmapGenerator = builtinPlugin
                .LookupGenerator(TxPluginResourceLocator.Of("tx:///:BrightnessBasedBitmapGenerator"))
                .Factory(TxArguments.Empty);
            var textBitmapRenderer = builtinPlugin
                .LookupRenderer(TxPluginResourceLocator.Of("tx:///:FontBitmapRenderer"))
                .Factory(TxArguments.Empty);

            var resizedBitmaps = resizeProcessor.Process(bitmaps);
            var textBitmaps = textBitmapGenerator.GenerateAsync(resizedBitmaps);
            await textBitmapRenderer.RenderAsync(textBitmaps, output);

            static async IAsyncEnumerable<T> OneAsync<T>(T value)
            {
                yield return await Task.Run(() => value);
            }
        }
    }
}
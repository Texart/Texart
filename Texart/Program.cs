using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SkiaSharp;
using Texart.Api;

namespace Texart
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            await using var output = File.OpenWrite("../../../../meme.gen.png");
            var bitmap = TxContract.NonNull(SKBitmap.Decode("../../../../meme.jpg"));
            ITxPlugin builtinPlugin = new Builtin.Plugin();

            var textBitmapGenerator = builtinPlugin
                .LookupGenerator(TxPluginResourceLocator.Of("tx:///:BrightnessBasedBitmapGenerator"))
                .Factory(TxArguments.Empty);
            var textBitmapRenderer = builtinPlugin
                .LookupRenderer(TxPluginResourceLocator.Of("tx:///:FontBitmapRenderer"))
                .Factory(TxArguments.Empty);

            var textBitmaps = textBitmapGenerator.GenerateAsync(OneAsync(bitmap));
            await textBitmapRenderer.RenderAsync(textBitmaps, output);

            static async IAsyncEnumerable<T> OneAsync<T>(T value)
            {
                yield return await Task.Run(() => value);
            }
        }
    }
}
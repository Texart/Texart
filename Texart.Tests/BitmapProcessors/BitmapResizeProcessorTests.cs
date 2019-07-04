using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using SkiaSharp;
using Texart.BitmapProcessors;

namespace Texart.Tests.BitmapProcessors
{
    [TestFixture]
    internal class BitmapResizeProcessorTests
    {
        [Test]
        public async Task CanResizeWithSameAspectRatio()
        {
            using var sourceBitmap = CreateSourceBitmap(1000, 1000, 200, 200);
            IBitmapProcessor<SKBitmap> resizeProcessor = new BitmapResizeProcessor(new SKSizeI(500, 500), SKFilterQuality.High);
            var resizedList = await CollectAsync(resizeProcessor.Process(OneAsync(sourceBitmap)));
            Assert.AreEqual(1, resizedList.Count);
            var resizedBitmap = resizedList[0];
            Assert.AreEqual(new SKSizeI(500, 500), new SKSizeI(resizedBitmap.Width, resizedBitmap.Height));

            // top left
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(200, 199));
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(199, 200));
            Assert.AreEqual(SKColors.Black, resizedBitmap.GetPixel(200, 200));

            // top right
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(299, 199));
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(300, 200));
            Assert.AreEqual(SKColors.Black, resizedBitmap.GetPixel(299, 200));

            // bottom left
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(200, 300));
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(199, 299));
            Assert.AreEqual(SKColors.Black, resizedBitmap.GetPixel(200, 299));

            // bottom right
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(299, 300));
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(300, 299));
            Assert.AreEqual(SKColors.Black, resizedBitmap.GetPixel(299, 299));
        }

        [Test]
        public async Task CanResizeWithoutPreservingAspectRatio()
        {
            using var sourceBitmap = CreateSourceBitmap(1000, 1000, 200, 200);
            IBitmapProcessor<SKBitmap> resizeProcessor = new BitmapResizeProcessor(new SKSizeI(500, 2000), SKFilterQuality.High);
            var resizedList = await CollectAsync(resizeProcessor.Process(OneAsync(sourceBitmap)));
            Assert.AreEqual(1, resizedList.Count);
            var resizedBitmap = resizedList[0];
            Assert.AreEqual(new SKSizeI(500, 2000), new SKSizeI(resizedBitmap.Width, resizedBitmap.Height));

            // Some artifacts may be created when stretching. A tolerance lets us test that the resizing "somewhat" worked, since
            // there are multiple ways to resize.
            const int artifactTolerancePixels = 5;

            // top left
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(200, 800 - artifactTolerancePixels));
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(199, 800 + artifactTolerancePixels));
            Assert.AreEqual(SKColors.Black, resizedBitmap.GetPixel(200, 800 + artifactTolerancePixels));

            // top right
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(299, 800 - artifactTolerancePixels));
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(300, 800 + artifactTolerancePixels));
            Assert.AreEqual(SKColors.Black, resizedBitmap.GetPixel(299, 800 + artifactTolerancePixels));

            // bottom left
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(200, 1199 + artifactTolerancePixels));
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(199, 1199 - artifactTolerancePixels));
            Assert.AreEqual(SKColors.Black, resizedBitmap.GetPixel(200, 1199 - artifactTolerancePixels));

            // bottom right
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(299, 1199 + artifactTolerancePixels));
            Assert.AreEqual(SKColors.White, resizedBitmap.GetPixel(300, 1199 - artifactTolerancePixels));
            Assert.AreEqual(SKColors.Black, resizedBitmap.GetPixel(299, 1199 - artifactTolerancePixels));
        }

        // black rectangle in the middle of a white background
        private static SKBitmap CreateSourceBitmap(int width, int height, float rectWidth, float rectHeight)
        {
            Debug.Assert(rectWidth <= width);
            Debug.Assert(rectHeight <= height);
            using var paint = new SKPaint
            {
                Color = SKColors.Black
            };
            var bitmap = new SKBitmap(
                width, width,
                SKImageInfo.PlatformColorType, SKAlphaType.Opaque
            );
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);
            canvas.DrawRect(
                (width - rectWidth) / 2f, (height - rectHeight) / 2f,
                rectWidth, rectHeight,
                paint);
            return bitmap;
        }

        private static async IAsyncEnumerable<T> OneAsync<T>(T value)
        {
            yield return await Task.Run(() => value);
        }

        private static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> asyncEnumerable)
        {
            var collector = new List<T>();
            await foreach (var item in asyncEnumerable)
            {
                collector.Add(item);
            }
            return collector;
        }
    }
}
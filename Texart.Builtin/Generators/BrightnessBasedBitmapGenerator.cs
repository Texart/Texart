using SkiaSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Texart.Builtin.Internal;
using Texart.Api;

namespace Texart.Builtin.Generators
{
    /// <summary>
    /// A generator that looks at the average brightness of N-by-N pixels to determine one character
    /// in the output text. The aggregation is done in parallel, completely on the CPU.
    /// </summary>
    internal sealed class BrightnessBasedBitmapGenerator : TxTextBitmapGeneratorBase, ITxTextBitmapGenerator
    {
        /// <inheritdocs/>
        public override Task<ITxTextBitmap> DoGenerateTextAsync(SKBitmap bitmap)
        {
            var characters = this.Characters;
            var charactersCount = characters.Count;
            var targetWidth = WidthFor(bitmap);
            var targetHeight = HeightFor(bitmap);
            var brightnessValues = this.GenerateBrightnessArray(bitmap);

            Debug.Assert(charactersCount > 0);
            Debug.Assert(brightnessValues.Length == targetWidth * targetHeight);

            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = -1 };

            // we preallocate the memory because I'm not so sure LINQ's #Select is smart
            // enough to do something like this
            var targetData = new char[brightnessValues.Length];
            Parallel.ForEach(brightnessValues, parallelOptions,
            (brightness, loopState, index) =>
            {
                Debug.Assert(brightness >= 0f && brightness <= 255f);
                // we scale the brightness to [0, charactersCount)
                float scaledBrightness = (brightness / 255f) * (charactersCount - 1);
                var scaledCharacterIndex = (int)(scaledBrightness + 0.5f);

                // we count from the end because the characters are sorted from brightest to darkest
                // so high brightness => lower index
                targetData[index] = characters[charactersCount - scaledCharacterIndex - 1];
            });

            ITxTextBitmap txTextBitmap = new TxArrayTxTextBitmap(targetData, targetWidth, targetHeight);
            return Task.FromResult(txTextBitmap);
        }

        /// <summary>
        /// Generates an array of brightness values (0-255) that represents the <see cref="SKBitmap"/>.
        /// The length of this array is <c>Width * Height</c>.
        /// </summary>
        /// <returns>An array representing the brightness of each chunk of <see cref="SKBitmap"/></returns>
        private float[] GenerateBrightnessArray(SKBitmap bitmap)
        {
            var sourceWidth = bitmap.Width;
            var sourceHeight = bitmap.Height;
            var targetWidth = WidthFor(bitmap);
            var targetHeight = HeightFor(bitmap);

            // we assume that we can perfectly scale source to target in square-sized chunks
            Debug.Assert(sourceWidth % targetWidth == 0);
            Debug.Assert(sourceHeight % targetHeight == 0);

            var pixelSamplingRatio = this.PixelSamplingRatio;
            var pixelsPerChunk = pixelSamplingRatio * pixelSamplingRatio;

            // stores the average brightness of each chunk
            var brightnessValues = new float[targetWidth * targetHeight];

            // process chunks in parallel
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = -1 };
            Parallel.For(0, targetWidth, parallelOptions, horizontalChunkIndex =>
            {
                // the x position of the pixel at the top left of this chunk
                var chunkX = horizontalChunkIndex * pixelSamplingRatio;
                Parallel.For(0, targetHeight, parallelOptions, verticalChunkIndex =>
                {
                    // the y position of the pixel at the top left of this chunk
                    var chunkY = verticalChunkIndex * pixelSamplingRatio;
                    // the current chunk's coordinate projected to an index for a 1D array
                    var chunkCoordinateProjectedIndex = verticalChunkIndex * targetWidth + horizontalChunkIndex;

                    // we iterate over this square chunk and accumulate the brightness
                    // value of each pixel. This chunk is almost like a bitmap in itself.
                    // We could to do a parallel accumulate, but we won't do that for now. Unless
                    // the Bitmap is huge or pixelSamplingRatio is tiny, each chunk should be small
                    // enough.
                    float accumulatedBrightness = 0f;
                    for (var offsetX = 0; offsetX < pixelSamplingRatio; ++offsetX)
                    {
                        for (var offsetY = 0; offsetY < pixelSamplingRatio; ++offsetY)
                        {
                            SKColor pixelColor = bitmap.GetPixel(chunkX + offsetX, chunkY + offsetY);
                            float alphaFactor = (pixelColor.Alpha / 255f);
                            float rgbAverage = (pixelColor.Red + pixelColor.Green + pixelColor.Blue) / 3f;
                            accumulatedBrightness += rgbAverage * alphaFactor;
                        }
                    }
                    // set the chunk's value to the average
                    var averageBrightness = accumulatedBrightness / pixelsPerChunk;
                    brightnessValues[chunkCoordinateProjectedIndex] = averageBrightness;
                });
            });

            return brightnessValues;
        }

        /// <summary>
        /// Constructs a generator with the given character set and <see cref="ITxTextBitmapGenerator.PixelSamplingRatio"/>.
        /// </summary>
        /// <param name="characters">The set of characters to use in the generation.</param>
        /// <param name="pixelSamplingRatio">See <see cref="ITxTextBitmapGenerator.PixelSamplingRatio"/>.</param>
        public BrightnessBasedBitmapGenerator(IEnumerable<char> characters, int pixelSamplingRatio = 1)
            : base(new List<char>(characters), pixelSamplingRatio)
        {
        }

        /// <summary>
        /// Factory function for <see cref="Plugin"/>.
        /// </summary>
        /// <param name="args">Input arguments.</param>
        /// <returns>Constructed instance.</returns>
        public static BrightnessBasedBitmapGenerator Create(TxArguments args)
        {
            // TODO: Use arguments
            return new BrightnessBasedBitmapGenerator(CharacterSets.Basic, 1);
        }

    }
}

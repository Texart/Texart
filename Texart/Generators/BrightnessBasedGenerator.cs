using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Texart.Generators
{
    public sealed class BrightnessBasedGenerator : TextGeneratorBase, ITextGenerator
    {
        /// <inheritdocs/>
        public override IList<char> Characters { get; }

        /// <inheritdocs/>
        public override ITextData GenerateText()
        {
            var characters = Characters;
            var charactersCount = Characters.Count;
            var targetWidth = Width;
            var targetHeight = Height;
            var brightnessValues = GenerateBrightnessArray();

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

                targetData[index] = characters[scaledCharacterIndex];
            });

            return new ArrayTextData(targetData, targetWidth, targetHeight);
        }

        /// <summary>
        /// Generates an array of brightness values (0-255) that represents the <code>Bitmap</code>.
        /// The length of this array is <code>Width * Height</code>.
        /// </summary>
        /// <returns>An array representing the brightness of each chunk of <code>Bitmap</code></returns>
        /// <see cref="Bitmap"/>
        private float[] GenerateBrightnessArray()
        {
            var sourceWidth = Bitmap.Width;
            var sourceHeight = Bitmap.Height;
            var targetWidth = Width;
            var targetHeight = Height;

            // we assume that we can perfectly scale source to target in square-sized chunks
            Debug.Assert(sourceWidth % targetWidth == 0);
            Debug.Assert(sourceHeight % targetHeight == 0);

            var pixelSamplingRatio = PixelSamplingRatio;
            var pixelsPerChunk = pixelSamplingRatio * pixelSamplingRatio;

            // stores the average brightness of each chunk
            var brightnessValues = new float[targetWidth * targetHeight];

            // process chunks in parallel
            using (var lockedBitmapAccessor = LockedBitmap(Bitmap))
            {
                SKBitmap bitmap = lockedBitmapAccessor.Bitmap;

                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = -1 };
                Parallel.For(0, targetWidth, parallelOptions, horizChunkIndex =>
                {
                    // the x position of the pixel at the top left of this chunk
                    var chunkX = horizChunkIndex * pixelSamplingRatio;
                    Parallel.For(0, targetHeight, parallelOptions, vertChunkIndex =>
                    {
                        // the y position of the pixel at the top left of this chunk
                        var chunkY = vertChunkIndex * pixelSamplingRatio;
                        // the current chunk's coordinate projected to an index for a 1D array
                        var chunkCoordProjectedIndex = vertChunkIndex * targetWidth + horizChunkIndex;

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
                        brightnessValues[chunkCoordProjectedIndex] = averageBrightness;
                    });
                });
            }

            return brightnessValues;
        }

        public BrightnessBasedGenerator(SKBitmap bitmap, IEnumerable<char> characters, int pixelSamplingRatio = 1)
        {
            if (bitmap == null) { throw new ArgumentNullException(nameof(bitmap)); }
            Bitmap = bitmap;

            if (characters == null) { throw new ArgumentNullException(nameof(characters)); }
            Characters = new List<char>(characters);
            if (Characters.Count < 1)
            {
                throw new ArgumentException($"{nameof(characters)} must have at least 1 character.");
            }

            if (pixelSamplingRatio < 1)
            {
                throw new ArgumentException($"{nameof(pixelSamplingRatio)} must be at least 1.");
            }
            if (!(bitmap.Width % pixelSamplingRatio == 0) || !(bitmap.Height % pixelSamplingRatio == 0))
            {
                throw new ArgumentException($"{nameof(pixelSamplingRatio)} must evenly divide both Bitmap width and height.");
            }
            PixelSamplingRatio = pixelSamplingRatio;
        }

    }
}

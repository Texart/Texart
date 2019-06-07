using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Texart.Api;

namespace Texart.Builtin.Renderers
{
    /// <summary>
    /// A renderer that paints text to an image using a provided font.
    /// </summary>
    internal sealed class FontBitmapRenderer : ITxTextBitmapRenderer
    {
        /// <summary>
        /// The typeface to paint with.
        /// </summary>
        /// <seealso cref="SKPaint.Typeface"/>
        private SKTypeface Typeface { get; }

        /// <summary>
        /// Determines if the output image should be antialiased.
        /// </summary>
        /// <seealso cref="SKPaint.IsAntialias"/>
        private bool ShouldAntialias => true;

        /// <summary>
        /// Determines if the output image should be dithered.
        /// </summary>
        /// <seealso cref="SKPaint.IsDither"/>
        private bool ShouldDither => true;

        /// <summary>
        /// Determines if font hinting is enabled.
        /// </summary>
        /// <seealso cref="SKPaint.IsAutohinted"/>
        private bool ShouldHint => true;

        /// <summary>
        /// The text font color.
        /// </summary>
        /// <seealso cref="SKPaint.Color"/>
        private SKColor ForegroundColor => SKColors.Black;

        /// <summary>
        /// The image background color. This is in contrast to <see cref="ForegroundColor"/>.
        /// </summary>
        private SKColor BackgroundColor => SKColors.White;

        /// <summary>
        /// The point size of the font.
        /// </summary>
        /// <seealso cref="SKPaint.TextSize"/>
        public float TextSize => 12f;

        /// <summary>
        /// The amount of spacing reserved for one character in the text data.
        /// Each character is assigned a square grid of length <c>CharacterSpacing</c> points.
        /// </summary>
        private int CharacterSpacing => 8;

        /// <inheritdocs />
        public async Task RenderAsync(IAsyncEnumerable<ITxTextBitmap> textBitmaps, Stream outputStream)
        {
            Debug.Assert(textBitmaps != null);
            Debug.Assert(outputStream != null);
            var didOutput = false;
            await foreach (var textBitmap in textBitmaps)
            {
                // TODO: Implement tiling when multiple images (and helper types in Texart.Api)
                if (didOutput)
                {
                    throw new InvalidOperationException("Only one output can be created");
                }
                using SKBitmap bitmap = GenerateBitmap(textBitmap);
                using SKImage image = SKImage.FromBitmap(bitmap);
                image.Encode().SaveTo(outputStream);
                didOutput = true;
            }
        }

        /// <summary>
        /// Generates a bitmap using the provided textBitmap and font info. Note that
        /// you are responsible for calling <see cref="IDisposable.Dispose"/> on the returned bitmap.
        /// </summary>
        /// <param name="txTextBitmap">The <see cref="ITxTextBitmap"/> to read from.</param>
        /// <returns>The generated <see cref="SKBitmap"/>.</returns>
        private SKBitmap GenerateBitmap(ITxTextBitmap txTextBitmap)
        {
            using var paint = new SKPaint
            {
                IsAntialias = ShouldAntialias,
                IsDither = ShouldDither,
                IsAutohinted = ShouldHint,
                Typeface = Typeface,
                TextSize = TextSize,
                TextEncoding = SKTextEncoding.Utf8,
                SubpixelText = true,
                DeviceKerningEnabled = false,
                Color = ForegroundColor
            };

            var backgroundColor = BackgroundColor;

            int textWidth = txTextBitmap.Width;
            int textHeight = txTextBitmap.Height;

            // spacing reserved for a single character
            SKFontMetrics fontMetrics = paint.FontMetrics;
            int characterSpacing = CharacterSpacing;

            Debug.Assert(characterSpacing > 0);

            // bitmap may not be big enough for all text if using
            // non-monospace characters and/or characterSize is not
            // sufficient. Too bad.
            int bitmapWidth = characterSpacing * textWidth;
            int bitmapHeight = characterSpacing * textHeight;

            Debug.Assert(bitmapWidth > 0);
            Debug.Assert(bitmapHeight > 0);

            // NOTE: this will need to be disposed by the caller.
            var bitmap = new SKBitmap(
                bitmapWidth, bitmapHeight,
                SKImageInfo.PlatformColorType, SKAlphaType.Premul
            );
            using (var canvas = new SKCanvas(bitmap))
            {
                bitmap.Erase(backgroundColor);
                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 1 };
                Parallel.For(0, textHeight, parallelOptions, y =>
                {
                    Parallel.For(0, textWidth, x =>
                    {
                        string charAsString = txTextBitmap.CharAt(x, y).ToString();

                        // dimensions of actual printed chars
                        float charWidth = paint.MeasureText(charAsString);
                        float charHeight = -fontMetrics.Ascent;
                        Debug.Assert(charWidth > 0);
                        Debug.Assert(charHeight > 0);

                        // the actual position to render them.
                        // they should be centered in the space allocated to them.
                        float textX = (x * characterSpacing) + (characterSpacing - charWidth) * 0.5f;
                        float textY = (y * characterSpacing) + (characterSpacing * 0.75f);

                        canvas.DrawText(
                            text: txTextBitmap.CharAt(x, y).ToString(),
                            x: textX,
                            y: textY,
                            paint: paint
                        );
                    });
                });
            }
            return bitmap;
        }

        /// <summary>
        /// Constructs a renderer with the given font.
        /// </summary>
        /// <param name="typeface">The typeface to use.</param>
        private FontBitmapRenderer(SKTypeface typeface)
        {
            Typeface = typeface ?? throw new ArgumentNullException(nameof(typeface));
        }

        /// <summary>
        /// Factory function for <see cref="Plugin"/>.
        /// </summary>
        /// <param name="args">Input arguments.</param>
        /// <returns>Constructed instance.</returns>
        public static FontBitmapRenderer Create(TxArguments args)
        {
            // TODO: use args
            var typeface = TxContract.NonNull(SKTypeface.FromFamilyName("Consolas"));
            return new FontBitmapRenderer(typeface);
        }
    }
}

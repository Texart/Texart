using SkiaSharp;
using System;
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
        /// The font to paint with, including metadata such as size, color, and spacing.
        /// </summary>
        public TxFont Font { get; }

        /// <summary>
        /// Determines if the output image should be antialiased.
        /// </summary>
        /// <see cref="SKPaint.IsAntialias"/>
        public bool ShouldAntialias { get; set; }

        /// <summary>
        /// Determines if the output image should be dithered.
        /// </summary>
        /// <see cref="SKPaint.IsDither"/>
        public bool ShouldDither { get; set; }

        /// <summary>
        /// Determines if font hinting is enabled.
        /// </summary>
        /// <see cref="SKPaint.IsAutohinted"/>
        public bool ShouldHint { get; set; }

        /// <summary>
        /// The image background color. This is in contrast to <see cref="TxFont.Color"/>.
        /// </summary>
        public SKColor BackgroundColor { get; set; } = DefaultBackgroundColor;
        /// <summary>
        /// The default image background color.
        /// </summary>
        public static SKColor DefaultBackgroundColor => SKColors.White;

        /// <inheritdocs />
        public Task RenderAsync(ITxTextBitmap txTextBitmap, Stream outputStream)
        {
            Debug.Assert(txTextBitmap != null);
            Debug.Assert(outputStream != null);

            using (SKBitmap bitmap = GenerateBitmap(txTextBitmap))
            using (SKImage image = SKImage.FromBitmap(bitmap))
            {
                image.Encode().SaveTo(outputStream);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Generates a bitmap using the provided textBitmap and font info. Note that
        /// you are responsible for calling <see cref="IDisposable.Dispose"/> on the returned bitmap.
        /// </summary>
        /// <param name="txTextBitmap">The <see cref="ITxTextBitmap"/> to read from.</param>
        /// <returns>The generated <see cref="SKBitmap"/>.</returns>
        private SKBitmap GenerateBitmap(ITxTextBitmap txTextBitmap)
        {
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = ShouldAntialias;
                paint.IsDither = ShouldDither;
                paint.IsAutohinted = ShouldHint;

                var font = Font;
                paint.Typeface = font.Typeface;
                paint.TextSize = font.TextSize;
                paint.TextEncoding = SKTextEncoding.Utf8;
                paint.SubpixelText = true;
                paint.DeviceKerningEnabled = false;

                paint.Color = font.Color;
                var backgroundColor = BackgroundColor;

                int textWidth = txTextBitmap.Width;
                int textHeight = txTextBitmap.Height;

                // spacing reserved for a single character
                SKFontMetrics fontMetrics = paint.FontMetrics;
                int characterSpacing = font.CharacterSpacing;

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
        }

        /// <summary>
        /// Constructs a renderer with the given font.
        /// </summary>
        /// <param name="txFont">The font to use.</param>
        public FontBitmapRenderer(TxFont txFont)
        {
            if (txFont == null) { throw new ArgumentNullException(nameof(txFont)); }
            if (txFont.Typeface == null) { throw new ArgumentNullException(nameof(txFont.Typeface)); }
            Font = txFont;
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
            return new FontBitmapRenderer(TxFont.FromTypeface(typeface));
        }
    }
}

using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Texart.Api;

namespace Texart.Builtin.Renderers
{
    public sealed class FontBitmapRenderer : ITextBitmapRenderer
    {
        /// <summary>
        /// The font to paint with.
        /// </summary>
        public Font Font { get; }

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

        public SKColor BackgroundColor { get; set; } = DefaultBackgroundColor;

        /// <inheritdocs />
        public Task RenderAsync(ITextBitmap textBitmap, Stream outputStream)
        {
            Debug.Assert(textBitmap != null);
            Debug.Assert(outputStream != null);

            using (SKBitmap bitmap = GenerateBitmap(textBitmap))
            using (SKImage image = SKImage.FromBitmap(bitmap))
            {
                image.Encode().SaveTo(outputStream);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Generates a bitmap using the provided textBitmap and font info. Note that
        /// you are responsible for calling <code>Dispose</code> on the returned bitmap.
        /// </summary>
        /// <param name="textBitmap">The <code>ITextBitmap</code> to read from</param>
        /// <returns>The generated <code>SKBitmap</code></returns>
        private SKBitmap GenerateBitmap(ITextBitmap textBitmap)
        {
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = this.ShouldAntialias;
                paint.IsDither = this.ShouldDither;
                paint.IsAutohinted = this.ShouldHint;

                var font = this.Font;
                paint.Typeface = font.Typeface.SkiaTypeface;
                paint.TextSize = font.TextSize;
                paint.TextEncoding = SKTextEncoding.Utf8;
                paint.SubpixelText = true;
                paint.DeviceKerningEnabled = false;

                paint.Color = font.Color;
                var backgroundColor = this.BackgroundColor;

                int textWidth = textBitmap.Width;
                int textHeight = textBitmap.Height;

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
                            string charAsString = textBitmap[x, y].ToString();

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
                                text: textBitmap[x, y].ToString(),
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

        public FontBitmapRenderer(Font font)
        {
            if (font == null) { throw new ArgumentNullException(nameof(font)); }
            if (font.Typeface == null) { throw new ArgumentNullException(nameof(font.Typeface)); }
            this.Font = font;
        }

        public static SKColor DefaultBackgroundColor => SKColors.White;

        public static FontBitmapRenderer Create(Lazy<JToken> json)
        {
            // TODO: use json
            return new FontBitmapRenderer(Api.Font.FromTypeface(Typeface.FromName("Consolas")));
        }
    }
}

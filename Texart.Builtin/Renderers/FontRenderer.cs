using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Texart.Builtin.Renderers
{
    public sealed class FontRenderer : ITextRenderer
    {
        /// <summary>
        /// The typeface to paint with.
        /// </summary>
        public SKTypeface Typeface { get; }

        /// <summary>
        /// The amount of spacing reserved for one character in the text data.
        /// That is, each character is assigned a square grid of length
        /// <code>CharacterSpacing</code>.
        /// </summary>
        public int CharacterSpacing { get; }

        /// <summary>
        /// Determines if the output image should be antialiased.
        /// </summary>
        /// <see cref="SKPaint.IsAntialias"/>
        public bool ShouldAntialias { get; }

        /// <summary>
        /// Determines if the output image should be dithered.
        /// </summary>
        /// <see cref="SKPaint.IsDither"/>
        public bool ShouldDither { get; }

        /// <summary>
        /// Determines if font hinting is enabled.
        /// </summary>
        /// <see cref="SKPaint.IsAutohinted"/>
        public bool ShouldHint { get; }

        /// <summary>
        /// The point size of the font.
        /// </summary>
        /// <see cref="SKPaint.TextSize"/>
        public float TextSize { get; }

        /// <inheritdocs />
        public Task RenderAsync(ITextData textData, Stream outputStream)
        {
            Debug.Assert(textData != null);
            Debug.Assert(outputStream != null);

            using (SKBitmap bitmap = GenerateBitmap(textData))
            using (SKImage image = SKImage.FromBitmap(bitmap))
            {
                image.Encode().SaveTo(outputStream);
            }

            // TODO: change this when we move to .NET 4.6
            return Task.FromResult(0);
        }

        /// <summary>
        /// Generates a bitmap using the provided textData and font info. Note that
        /// you are responsible for calling <code>Dispose</code> on the returned bitmap.
        /// </summary>
        /// <param name="textData">The <code>ITextData</code> to read from</param>
        /// <returns>The generated <code>SKBitmap</code></returns>
        public SKBitmap GenerateBitmap(ITextData textData)
        {
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = this.ShouldAntialias;
                paint.IsDither = this.ShouldDither;
                paint.IsAutohinted = this.ShouldHint;
                
                paint.Typeface = this.Typeface;
                paint.TextSize = this.TextSize;
                paint.TextEncoding = SKTextEncoding.Utf8;
                paint.SubpixelText = true;
                paint.DeviceKerningEnabled = false;

                int textWidth = textData.Width;
                int textHeight = textData.Height;

                // spacing reserved for a single character
                SKFontMetrics fontMetrics = paint.FontMetrics;
                int characterSpacing = this.CharacterSpacing;

                Debug.Assert(characterSpacing > 0);

                // bitmap may not be big enough for all text if using
                // non-monospace characters and/or characterSize/ is not
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
                    var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 1 };
                    Parallel.For(0, textHeight, parallelOptions, y =>
                    {
                        Parallel.For(0, textWidth, x =>
                        {
                            string charAsString = textData[x, y].ToString();

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
                                text: textData[x, y].ToString(),
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

        public FontRenderer(
            SKTypeface typeface,
            float textSize, int characterSpacing,
            bool antialias = true,
            bool dither = true,
            bool hint = true)
        {
            if (typeface == null) { throw new ArgumentNullException(nameof(typeface)); }
            if (textSize <= 0f) { throw new ArgumentException($"{nameof(textSize)} must be positive"); }
            if (characterSpacing <= 0) { throw new ArgumentException($"{nameof(characterSpacing)} must be positive"); }

            this.Typeface = typeface;
            this.TextSize = textSize;
            this.CharacterSpacing = characterSpacing;
            this.ShouldAntialias = antialias;
            this.ShouldDither = dither;
            this.ShouldHint = hint;
        }
    }
}

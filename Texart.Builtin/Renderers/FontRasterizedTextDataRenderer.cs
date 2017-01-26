using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Texart.Builtin.Renderers
{
    public class FontRasterizedTextDataRenderer : ITextDataRenderer
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
        /// Determines if font kerning is enabled.
        /// </summary>
        /// <see cref="SKPaint.DeviceKerningEnabled"/>
        public bool Kerning { get; }

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
        /// <param name="textData">The <code>TextData</code> to read from</param>
        /// <returns>The generated <code>SKBitmap</code></returns>
        public SKBitmap GenerateBitmap(ITextData textData)
        {
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.IsDither = true;
                paint.IsAutohinted = true;
                paint.DeviceKerningEnabled = true;
                paint.Typeface = Typeface;
                paint.TextSize = 12f;
                paint.TextEncoding = SKTextEncoding.Utf8;
                paint.SubpixelText = true;

                int textWidth = textData.Width;
                int textHeight = textData.Height;

                // spacing reserved for a single character
                SKFontMetrics fontMetrics = paint.FontMetrics;
                int characterSpacing = 8;

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
                        // make a string for the current line in parallel
                        // var lineChars = new char[textWidth];
                        Parallel.For(0, textWidth, x =>
                        {
                            // lineChars[x] = textData[x, y];
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
                        // var line = new string(lineChars);

                        // Console.WriteLine(line);

                        /*canvas.DrawText(
                            text: line,
                            x: 0f,
                            y: 10 + y * characterHeight,
                            paint: paint
                        );*/
                    });
                }
                return bitmap;
            }
        }

        public FontRasterizedTextDataRenderer(SKTypeface typeface)
        {
            if (typeface == null) { throw new ArgumentNullException(nameof(typeface)); }
        }
    }
}

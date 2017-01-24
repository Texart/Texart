using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Texart.Renderers
{
    class FontRasterizedTextDataRenderer : ITextDataRenderer
    {
        public SKTypeface Typeface { get; }

        public void Render(ITextData textData, Stream outputStream)
        {
            Debug.Assert(textData != null);
            Debug.Assert(outputStream != null);

            using (SKBitmap bitmap = GenerateBitmap(textData))
            using (SKImage image = SKImage.FromBitmap(bitmap))
            {
                image.Encode().SaveTo(outputStream);
            }
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

        /// <summary>
        /// The string to use to guess font width when we can't figure it out using other methods.
        /// </summary>
        private const string FallbackMeasuringString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    }
}

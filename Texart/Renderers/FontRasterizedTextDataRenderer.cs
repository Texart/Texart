using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;

namespace Texart.Renderers
{
    class FontRasterizedTextDataRenderer : ITextDataRenderer
    {
        public void Render(ITextData textData, Stream outputStream)
        {
            Debug.Assert(textData != null);
            Debug.Assert(outputStream != null);
        }

        public SKBitmap GenerateBitmap(ITextData textData)
        {
            // SKCanvas x;
            // SKPaint p;
            // p.FontMetrics.
            return null;
        }

        public FontRasterizedTextDataRenderer(SKTypeface typeface)
        {
            if (typeface == null) { throw new ArgumentNullException(nameof(typeface)); }
        }
    }
}

using Texart.Builtin.Renderers;

namespace Texart.ScriptInterface
{
    public static partial class Tx
    {
        /// <summary>
        /// Creates a renderer that uses a <code>Font</code> to render a bitmap.
        /// </summary>
        /// <param name="font">The font to render with.</param>
        /// <param name="antialias">Whether the image should be antialiased.</param>
        /// <param name="dither">Whether the font should be dithered.</param>
        /// <param name="hint">Whether the font should be hinted.</param>
        /// <returns>A renderer using a font.</returns>
        public static ITextRenderer CreateFontRenderer(
            Font font,
            bool antialias = true,
            bool dither = true,
            bool hint = true)
        {
            return new FontRenderer(
                typeface: font.Typeface.SkiaTypeface,
                textSize: font.TextSize,
                characterSpacing: font.DesiredCharacterSpacing,
                antialias: antialias,
                dither: dither,
                hint: hint
            );
        }
    }
}

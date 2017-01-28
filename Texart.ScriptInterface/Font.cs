using SkiaSharp;

namespace Texart.ScriptInterface
{
    public class Font
    {
        /// <summary>
        /// The desired number of pixels that represents the length
        /// of the bounding box for one character.
        /// </summary>
        public int DesiredCharacterSpacing { get; set; }
        /// <summary>
        /// The desired font size.
        /// </summary>
        public float TextSize { get; set; }
        /// <summary>
        /// The underlying typeface.
        /// </summary>
        public Typeface Typeface { get; set; }

        public Color Color { get; set; }

        public static Font FromTypeface(Typeface typeface)
        {
            return new Font()
            {
                Typeface = typeface,
                DesiredCharacterSpacing = DefaultCharacterSpacing,
                TextSize = DefaultTextSize,
                Color = DefaultColor
            };
        }

        public static int DefaultCharacterSpacing { get { return 8; } }
        public static float DefaultTextSize { get { return 12f; } }
        public static Color DefaultColor { get { return new Color(SKColors.Black); } }
    }
}

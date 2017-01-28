using SkiaSharp;
using System;

namespace Texart.Interface
{
    public class Font
    {
        /// <summary>
        /// The amount of spacing reserved for one character in the text data.
        /// That is, each character is assigned a square grid of length
        /// <code>CharacterSpacing</code>.
        /// </summary>
        public int CharacterSpacing
        {
            get { return this._characterSpacing; }
            set
            {
                if (value <= 0) { throw new ArgumentException($"{nameof(CharacterSpacing)} must be positive"); }
                this._characterSpacing = value;
            }
        }
        private int _characterSpacing = DefaultCharacterSpacing;

        /// <summary>
        /// The point size of the font.
        /// </summary>
        /// <see cref="SKPaint.TextSize"/>
        public float TextSize
        {
            get { return this._textSize; }
            set
            {
                if (value <= 0f) { throw new ArgumentException($"{nameof(this.TextSize)} must be positive"); }
                this._textSize = value;
            }
        }
        private float _textSize = DefaultTextSize;
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
                CharacterSpacing = DefaultCharacterSpacing,
                TextSize = DefaultTextSize,
                Color = DefaultColor
            };
        }

        public static int DefaultCharacterSpacing { get { return 8; } }
        public static float DefaultTextSize { get { return 12f; } }
        public static Color DefaultColor { get { return new Color(SKColors.Black); } }
    }
}

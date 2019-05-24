using SkiaSharp;
using System;

namespace Texart.Api
{
    /// <summary>
    /// A <code>TxFont</code> represents a particular <code>SKTypeface</code> with extra
    /// properties such as font size, color, and spacing.
    /// For example: "Garamond 12pt Bold", "Courier New 8pt Italic".
    /// </summary>
    /// <see cref="SKTypeface"/>
    public class TxFont
    {
        /// <summary>
        /// Gets or sets the amount of spacing reserved for one character in the text data.
        /// That is, each character is assigned a square grid of length
        /// <code>CharacterSpacing</code>.
        /// </summary>
        /// <see cref="DefaultCharacterSpacing"/>
        /// <exception cref="ArgumentException">If trying to set a value of less than or equal to <code>0</code></exception>
        public int CharacterSpacing
        {
            get => this._characterSpacing;
            set
            {
                if (value <= 0) { throw new ArgumentException($"{nameof(CharacterSpacing)} must be positive"); }
                this._characterSpacing = value;
            }
        }
        private int _characterSpacing = DefaultCharacterSpacing;

        /// <summary>
        /// Gets or sets the point size of the font.
        /// </summary>
        /// <see cref="SKPaint.TextSize"/>
        /// <see cref="DefaultTextSize"/> 
        /// <exception cref="ArgumentException">If trying to set a value of less than or equal to <code>0f</code></exception>
        public float TextSize
        {
            get => this._textSize;
            set
            {
                if (value <= 0f) { throw new ArgumentException($"{nameof(this.TextSize)} must be positive"); }
                this._textSize = value;
            }
        }
        private float _textSize = DefaultTextSize;

        /// <summary>
        /// Gets or sets the underlying typeface.
        /// </summary>
        public SKTypeface Typeface { get; set; }

        /// <summary>
        /// Gets or sets the foreground color of this font.
        /// </summary>
        /// <see cref="DefaultColor"/>
        public SKColor Color { get; set; }

        /// <summary>
        /// Creates a <code>Font</code> with default values with the given typeface.
        /// </summary>
        /// <param name="typeface">The typeface to create a font from.</param>
        /// <returns>A new <code>Font</code> with the given typeface.</returns>
        public static TxFont FromTypeface(SKTypeface typeface)
        {
            return new TxFont()
            {
                Typeface = typeface,
                CharacterSpacing = DefaultCharacterSpacing,
                TextSize = DefaultTextSize,
                Color = DefaultColor
            };
        }

        /// <summary>
        /// Gets the default value for character spacing.
        /// </summary>
        /// <see cref="CharacterSpacing"/>
        public static int DefaultCharacterSpacing => 8;

        /// <summary>
        /// Gets the default value for text size.
        /// </summary>
        /// <see cref="TextSize"/>
        public static float DefaultTextSize => 12f;

        /// <summary>
        /// Gets the default value for color.
        /// </summary>
        /// <see cref="Color"/>
        public static SKColor DefaultColor => SKColors.Black;
    }
}

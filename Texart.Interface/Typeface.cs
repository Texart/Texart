using SkiaSharp;

namespace Texart.Api
{
    /// <summary>
    /// A <code>Typeface</code> represents a generic typeface as in typography.
    /// For example: "Consolas", "Times New Roman". A typeface does not define text
    /// sizes or weights.
    /// </summary>
    /// <see cref="Font"/>
    public class Typeface
    {
        /// <summary>
        /// The internal reference to the underlying <code>SKTypeface</code>.
        /// </summary>
        [SkiaProperty]
        public SKTypeface SkiaTypeface { get; set; }

        /// <summary>
        /// Creates a <code>Typeface</code> from the provided file name.
        /// </summary>
        /// <param name="fileName">The file name to load from.</param>
        /// <param name="index">The index of the typeface to load from the given file.</param>
        /// <returns></returns>
        public static Typeface FromFile(string fileName, int index = 0)
        {
            var skiaTypeface = SKTypeface.FromFile(fileName, index);
            if (skiaTypeface == null) { throw new System.ArgumentException($"Could not load typeface from the file '{fileName}'"); }
            return new Typeface() { SkiaTypeface = skiaTypeface };
        }

        /// <summary>
        /// Creates a <code>Typeface</code> from the provided font family name. If an exact match is
        /// not found, then the closest match is returned.
        /// </summary>
        /// <param name="fontName">The font family name.</param>
        /// <returns></returns>
        public static Typeface FromName(string fontName)
        {
            var skiaTypeface = SKTypeface.FromFamilyName(fontName);
            if (skiaTypeface == null) { throw new System.ArgumentException($"Could not load typeface from the name '{fontName}'"); }
            return new Typeface() { SkiaTypeface = skiaTypeface };
        }

        /// <summary>
        /// Private constructor. Does nothing.
        /// </summary>
        private Typeface() { }

        ~Typeface()
        {
            this.SkiaTypeface.Dispose();
        }
    }
}

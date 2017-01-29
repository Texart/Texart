using SkiaSharp;

namespace Texart.Interface
{
    /// <summary>
    /// A <code>Color</code> represents a 32-bit RGBA color.
    /// </summary>
    public struct Color
    {
        /// <summary>
        /// The underlying color. Note that this should only be used when implementing
        /// generators and renderers.
        /// </summary>
        [SkiaProperty]
        public SKColor SkiaColor { get; set; }

        /// <summary>
        /// Creates a new <code>Color</code> with the given channel values.
        /// </summary>
        /// <param name="r">The Red channel.</param>
        /// <param name="g">The Green channel.</param>
        /// <param name="b">The Blue channel.</param>
        /// <param name="a">The Alpha channel.</param>
        public Color(byte r, byte g, byte b, byte a = 255)
        {
            this.SkiaColor = new SKColor(r, g, b, a);
        }

        /// <summary>
        /// Creates a <code>Color</code> directly given the underlying skia color.
        /// </summary>
        /// <param name="color">The underlying color.</param>
        internal Color(SKColor color)
        {
            this.SkiaColor = color;
        }

        /// <summary>
        /// Gets or sets the Red channel of the color.
        /// </summary>
        byte R
        {
            get { return this.SkiaColor.Red; }
            set { this.SkiaColor = this.SkiaColor.WithRed(value); }
        }

        /// <summary>
        /// Gets or sets the Green channel of the color.
        /// </summary>
        byte G
        {
            get { return this.SkiaColor.Green; }
            set { this.SkiaColor = this.SkiaColor.WithGreen(value); }
        }

        /// <summary>
        /// Gets or sets the Blue channel of the color.
        /// </summary>
        byte B
        {
            get { return this.SkiaColor.Blue; }
            set { this.SkiaColor = this.SkiaColor.WithBlue(value); }
        }

        /// <summary>
        /// Gets or sets the Alpha channel of the color.
        /// </summary>
        byte A
        {
            get { return this.SkiaColor.Alpha; }
            set { this.SkiaColor = this.SkiaColor.WithAlpha(value); }
        }

        /// <summary>
        /// Returns a new <code>Color</code> with the same channels as this one
        /// with the Red channel replaced with the given value.
        /// </summary>
        /// <param name="value">The Red channel.</param>
        /// <returns>A new <code>Color</code> with the given Red channel.</returns>
        public Color WithRed(byte value)
        {
            return new Color() { SkiaColor = this.SkiaColor.WithRed(value) };
        }

        /// <summary>
        /// Returns a new <code>Color</code> with the same channels as this one
        /// with the Green channel replaced with the given value.
        /// </summary>
        /// <param name="value">The Green channel.</param>
        /// <returns>A new <code>Color</code> with the given Green channel.</returns>
        public Color WithGreen(byte value)
        {
            return new Color() { SkiaColor = this.SkiaColor.WithGreen(value) };
        }

        /// <summary>
        /// Returns a new <code>Color</code> with the same channels as this one
        /// with the Blue channel replaced with the given value.
        /// </summary>
        /// <param name="value">The Blue channel.</param>
        /// <returns>A new <code>Color</code> with the given Blue channel.</returns>
        public Color WithBlue(byte value)
        {
            return new Color() { SkiaColor = this.SkiaColor.WithBlue(value) };
        }

        /// <summary>
        /// Returns a new <code>Color</code> with the same channels as this one
        /// with the Blue channel replaced with the given value.
        /// </summary>
        /// <param name="value">The Alpha channel.</param>
        /// <returns>A new <code>Color</code> with the given Alpha channel.</returns>
        public Color WithAlpha(byte value)
        {
            return new Color() { SkiaColor = this.SkiaColor.WithAlpha(value) };
        }
    }
}

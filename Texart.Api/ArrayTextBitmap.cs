using System;
using System.Diagnostics;

namespace Texart.Api
{
    /// <summary>
    /// An implementation of <code>ITextBitmap</code> based on an underlying
    /// one dimensional array.
    /// </summary>
    public sealed class ArrayTextBitmap : ITextBitmap
    {
        /// <inheritdoc/>
        public int Height { get; }
        /// <inheritdoc/>
        public int Width { get; }

        /// <summary>
        /// A one dimensional character array which represents the two dimensional
        /// image data.
        /// </summary>
        private char[] Characters { get; }

        /// <summary>
        /// Constructs an <code>ArrayTextBitmap</code> given the underlying array to use.
        /// The length of this array must match the provided <code>width</code> and
        /// <code>height</code>. That is, <code>characters.length == width * height</code>.
        /// </summary>
        /// <param name="characters">The underlying array</param>
        /// <param name="width">The width of the text data</param>
        /// <param name="height">The height of the text data</param>
        public ArrayTextBitmap(char[] characters, int width, int height)
        {
            // TODO: perhaps these checks should happen in Release too
            Debug.Assert(characters != null);
            Debug.Assert(width > 0);
            Debug.Assert(height > 0);
            Debug.Assert(characters.Length == width * height);

            Width = width;
            Height = height;
            Characters = characters;
        }

        /// <inheritdocs/>
        public char this[int x, int y]
        {
            get
            {
                CheckCoordinate(x, y);
                return Characters[y * Width + x];
            }
        }

        /// <summary>
        /// Asserts that the specified bounds are within the bounds of the underlying data.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        [Conditional("DEBUG")]
        private void CheckCoordinate(int x, int y)
        {
            if (x < 0 || x >= Width)
            {
                throw new ArgumentException($"{nameof(x)}={x} exceeds bound of data (Width={Width})");
            }
            if (y < 0 || y >= Height)
            {
                throw new ArgumentException($"{nameof(y)}={y} exceeds bound of data (Width={Height})");
            }
        }
    }
}

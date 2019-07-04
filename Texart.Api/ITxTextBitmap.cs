#nullable enable

namespace Texart.Api
{
    /// <summary>
    /// An <see cref="ITxTextBitmap"/> is a bitmap of character data – like <c>char[,]</c>.
    ///
    /// The API exposes an indexer-like method, <c>char ChatAt(int x, int y)</c> for 2D-array-like access.
    /// The internal representation is implementation-defined. The underlying computation can be performed
    /// on the fly, or wrap around a pre-populated <c>char[,]</c>. etc.
    ///
    /// Implementations are strongly encouraged to be immutable.
    /// </summary>
    public interface ITxTextBitmap
    {
        /// <summary>
        /// Gets the width of the text data.
        /// </summary>
        /// <see cref="CharAt(int, int)"/>
        int Width { get; }
        /// <summary>
        /// Gets the width of the text data.
        /// </summary>
        /// <see cref="CharAt(int, int)"/>
        int Height { get; }

        /// <summary>
        /// Gets the character (pixel) at the given x and y indices.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <returns>The character at the given position</returns>
        /// <see cref="Height"/>
        /// <see cref="Width"/>
        char CharAt(int x, int y);
    }
}

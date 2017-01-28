namespace Texart.Interface
{
    public interface ITextData
    {
        /// <summary>
        /// Gets the width of the text data.
        /// </summary>
        /// <see cref="this[int, int]"/>
        int Width { get; }
        /// <summary>
        /// Gets the width of the text data.
        /// </summary>
        /// <see cref="this[int, int]"/>
        int Height { get; }

        /// <summary>
        /// Gets the character (pixel) at the given x and y indices.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <returns>The character at the given position</returns>
        /// <see cref="Height"/>
        /// <see cref="Width"/>
        char this[int x, int y] { get; }
    }
}

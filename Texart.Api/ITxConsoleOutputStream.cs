using System;

#nullable enable

namespace Texart.Api
{
    /// <summary>
    /// An abstraction for standard output streams: <c>stdout</c>, and <c>stderr</c>.
    /// </summary>
    public interface ITxConsoleOutputStream
    {
        /// <summary>
        /// Write some characters to the output stream.
        /// </summary>
        /// <param name="value">The characters to write.</param>
        void Write(ReadOnlySpan<char> value);
    }
}
using System.Collections.Generic;

namespace Texart.Builtin.Internal
{
    /// <summary>
    /// Commonly used internal character sets.
    /// </summary>
    internal static class CharacterSets
    {
        /// <summary>
        /// Simple and short character set that is mostly linear with most fonts.
        /// </summary>
        internal static IEnumerable<char> Basic =>
            new[] {
                ' ', ' ', ' ', ' ',
                '.', '.',
                ',', ',',
                '-', '-', '-',
                '~', '~', '~',
                ':', ':',
                ';', ';', ';',
                '!', '!', '!',
                '*', '*', '*',
                '=', '=', '=', '=', '=',
                '$', '$', '$', '$', '$', '$', '$',
                '#', '#', '#', '#', '#', '#', '#', '#', '#', '#',
                '&', '&', '&', '&', '&', '&', '&', '&', '&', '&',
                '@', '@', '@', '@', '@', '@', '@', '@', '@', '@', '@', '@'
            };
    }
}

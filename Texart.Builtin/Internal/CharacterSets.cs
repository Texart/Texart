using System.Collections.Generic;

namespace Texart.Builtin.Internal
{
    public static class CharacterSets
    {
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

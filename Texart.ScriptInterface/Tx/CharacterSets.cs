using System.Collections.Generic;

namespace Texart.ScriptInterface
{
    public static partial class Tx
    {
        public static class CharacterSets
        {
            public static IEnumerable<char> Basic =>
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
}

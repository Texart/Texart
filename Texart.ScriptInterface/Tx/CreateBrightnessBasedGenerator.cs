using System.Collections.Generic;
using Texart.Builtin.Generators;

namespace Texart.ScriptInterface
{
    public static partial class Tx
    {
        public static BrightnessBasedGenerator CreateBrightnessBasedGenerator(IEnumerable<char> characterSet, int pixelRatio = 1)
        {
            return new BrightnessBasedGenerator(
                characters: characterSet,
                pixelSamplingRatio: pixelRatio
            );
        }
    }
}

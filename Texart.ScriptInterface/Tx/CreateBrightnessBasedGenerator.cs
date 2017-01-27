using System.Collections.Generic;
using Texart.Builtin.Generators;

namespace Texart.ScriptInterface
{
    public static partial class Tx
    {
        public static ITextGenerator BrightnessBasedGenerator(IEnumerable<char> characterSet, int samplingRatio = 1)
        {
            return new BrightnessBasedGenerator(
                characters: characterSet,
                pixelSamplingRatio: samplingRatio
            );
        }
    }
}

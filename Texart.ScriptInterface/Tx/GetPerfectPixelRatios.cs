using System;
using System.Collections.Generic;
using System.Diagnostics;
using Texart.Interface;

namespace Texart.ScriptInterface
{
    public static partial class Tx
    {
        public static IEnumerable<int> GetPerfectPixelRatios(Bitmap bitmap)
        {
            var width = bitmap.Width;
            var height = bitmap.Height;

            // The idea is to get every common factor between the width and the height

            Debug.Assert(width > 0);
            Debug.Assert(height > 0);

            // All common factors are <= the greatest common factor by definition
            int gcd = GetPerfectPixelRatios__Gcd(width, height);
            Debug.Assert(gcd >= 1);

            int checkMax = (int)Math.Sqrt(gcd);
            for (var possibleFactor = checkMax; possibleFactor >= 1; --possibleFactor)
            {
                if (gcd % possibleFactor == 0)
                {
                    // possibleFactor is indeed a factor!
                    var factor = possibleFactor;
                    yield return factor;
                    var complementaryFactor = gcd / factor;
                    // don't yield the same number twice
                    if (complementaryFactor != factor)
                    {
                        yield return complementaryFactor;
                    }
                }
            }
        }

        private static int GetPerfectPixelRatios__Gcd(int a, int b)
        {
            // Generic Euclidean GCD algorithm
            while (b != 0)
            {
                var temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
    }
}

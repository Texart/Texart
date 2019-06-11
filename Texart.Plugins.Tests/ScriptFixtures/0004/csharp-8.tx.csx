#r "SkiaSharp.dll"
#r "Texart.Api.dll"

using System.Collections.Generic;

async IAsyncEnumerable<int> GetInts() {
    yield return 0;
    yield return 1;
    yield return 2;
}

return GetInts();

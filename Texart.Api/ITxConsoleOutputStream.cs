using System;

#nullable enable

namespace Texart.Api
{
    public interface ITxConsoleOutputStream
    {
        void Write(ReadOnlySpan<char> value);
    }
}
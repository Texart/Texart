using System;

namespace Texart.Api
{
    public interface ITxConsoleOutputStream
    {
        void Write(ReadOnlySpan<char> value);
    }
}
namespace Texart.Api
{
    public interface ITxConsole
    {
        ITxConsoleOutputStream Out { get; }
        bool IsOutputRedirected { get; }

        ITxConsoleOutputStream Error { get; }
        bool IsErrorRedirected { get; }
    }
}
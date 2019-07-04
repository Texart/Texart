#nullable enable

namespace Texart.Api
{
    /// <summary>
    /// An abstraction for standard IO streams: <c>stdout</c>, <c>stderr</c>, and <c>stdin</c>.
    /// </summary>
    public interface ITxConsole
    {
        /// <summary>
        /// The standard output stream, <c>stdout</c>.
        /// </summary>
        ITxConsoleOutputStream Out { get; }
        /// <summary>
        /// Whether or not <see cref="Out"/> is redirected to somewhere other than the tty.
        /// </summary>
        bool IsOutputRedirected { get; }

        /// <summary>
        /// The standard error stream, <c>stderr</c>.
        /// </summary>
        ITxConsoleOutputStream Error { get; }
        /// <summary>
        /// Whether or not <see cref="Error"/> is redirected to somewhere other than the tty.
        /// </summary>
        bool IsErrorRedirected { get; }
    }
}
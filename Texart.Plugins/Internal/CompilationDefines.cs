namespace Texart.Plugins.Internal
{
    internal static class CompilationDefines
    {
        /// <summary>
        /// Whether or not this assembly is being compiled for Debug.
        /// </summary>
        public const bool IsDebug =
#if DEBUG
            true;
#else
            false;
#endif

        /// <summary>
        /// Whether or not this assembly is being compiled for Release.
        /// </summary>
        public const bool IsRelease = !IsDebug;
    }
}
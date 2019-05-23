using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Texart.Builtin
{
    internal static class PlatformHelpers
    {
        /// <summary>
        /// Determines if we are currently executing on a Unix system.
        /// <see cref="http://stackoverflow.com/questions/5116977/how-to-check-the-os-version-at-runtime-e-g-windows-or-linux-without-using-a-con"/>
        /// </summary>
        public static bool IsUnix =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        /// <summary>
        /// The default <code>Encoding</code> to use. This is ASCII on Windows and UTF-8 on Unices.
        /// </summary>
        public static Encoding DefaultEncoding => IsUnix ? Encoding.UTF8 : Encoding.ASCII;
    }
}

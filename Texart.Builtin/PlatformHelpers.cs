using System;
using System.Text;

namespace Texart.Builtin
{
    internal static class PlatformHelpers
    {
        /// <summary>
        /// Determines if we are currently executing on a Unix system.
        /// </summary>
        public static bool IsUnix
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                // http://stackoverflow.com/questions/5116977/how-to-check-the-os-version-at-runtime-e-g-windows-or-linux-without-using-a-con
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        /// <summary>
        /// The default <code>Encoding</code> to use. This is ASCII on Windows and UTF-8 on Unices.
        /// </summary>
        public static Encoding DefaultEncoding
        {
            get
            {
                return IsUnix ? Encoding.UTF8 : Encoding.ASCII;
            }
        }
    }
}

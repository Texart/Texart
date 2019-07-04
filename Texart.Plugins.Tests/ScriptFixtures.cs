using System;
using System.IO;
using System.Threading.Tasks;
using Texart.Api;
using Texart.Plugins.Scripting;

namespace Texart.Plugins.Tests
{
    /// <summary>
    /// Helper methods for accessing test script fixtures.
    /// </summary>
    internal static class ScriptFixtures
    {
        /// <summary>
        /// The base directory containing all fixtures.
        /// </summary>
        public static string BaseDirectory => Path.GetFullPath("../../../ScriptFixtures/");

        /// <summary>
        /// Gets the directory path where scripts for the provided fixture live.
        /// </summary>
        /// <param name="fixture">Fixture number.</param>
        /// <returns>Directory for provided fixture.</returns>
        public static string GetDirectory(int fixture)
        {
            CheckFixtureNumber(fixture);
            return Path.Combine(BaseDirectory, FormatFixturePath(fixture));
        }

        /// <summary>
        /// Gets the path relative to a fixture directory.
        /// </summary>
        /// <param name="fixture">Fixture number.</param>
        /// <param name="relativePath">The relative path from the fixture directory.</param>
        /// <returns></returns>
        public static string GetPath(int fixture, string relativePath)
        {
            if (Path.IsPathRooted(relativePath) || Path.IsPathFullyQualified(relativePath))
            {
                throw new ArgumentException($"{nameof(relativePath)} must be a relative path, received: {relativePath}");
            }

            return Path.Combine(GetDirectory(fixture), relativePath);
        }

        /// <summary>
        /// Loads a plugin script from the provided fixture and relative path.
        /// </summary>
        /// <param name="fixture">Fixture number.</param>
        /// <param name="relativePath">The relative path from the fixture directory.</param>
        /// <returns>Loaded script.</returns>
        public static async Task<PluginScriptCompilation<ITxPlugin>> LoadFrom(int fixture, string relativePath)
        {
            var path = GetPath(fixture, relativePath);
            if (!relativePath.EndsWith(ScriptingConstants.TexartScriptFileSuffix))
            {
                throw new ArgumentException($"Attempt to load a non Texart script file: {path}");
            }
            var pluginScript = PluginScript.LoadFrom(path);
            return await pluginScript.Compile();
        }

        /// <summary>
        /// Loads a plugin script from the provided fixture and relative path.
        /// </summary>
        /// <typeparam name="T">The return type of the script.</typeparam>
        /// <param name="fixture">Fixture number.</param>
        /// <param name="relativePath">The relative path from the fixture directory.</param>
        /// <returns>Loaded script.</returns>
        public static async Task<PluginScriptCompilation<T>> LoadFrom<T>(int fixture, string relativePath)
        {
            var path = GetPath(fixture, relativePath);
            if (!relativePath.EndsWith(ScriptingConstants.TexartScriptFileSuffix))
            {
                throw new ArgumentException($"Attempt to load a non Texart script file: {path}");
            }
            var pluginScript = PluginScript.LoadFrom<T>(path);
            return await pluginScript.Compile();
        }

        /// <summary>
        /// Verifies that the fixture number is in range.
        /// </summary>
        /// <param name="fixture">Fixture number.</param>
        private static void CheckFixtureNumber(int fixture)
        {
            if (fixture <= 0 || fixture > 1000)
            {
                throw new ArgumentException($"Only fixtures numbered 0000 to 1000 are supported, received: {fixture}");
            }
        }

        /// <summary>
        /// Formats the given fixture number as a directory name.
        /// </summary>
        /// <param name="fixture">Fixture number.</param>
        /// <returns>Formatted string.</returns>
        private static string FormatFixturePath(int fixture) => fixture.ToString("0000");
    }
}
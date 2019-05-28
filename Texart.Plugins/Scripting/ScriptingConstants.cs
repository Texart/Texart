using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Texart.Plugins.Tests")]
namespace Texart.Plugins.Scripting
{
    /// <summary>
    /// Helper class containing constants floating around the scripting logic implementation.
    /// </summary>
    internal static class ScriptingConstants
    {
        /// <summary>
        /// The Texart Api assembly file name. For types like <see cref="Texart.Api.IPlugin"/>.
        /// Used by <see cref="Diagnostics.RequiredReferencesDirectiveAnalyzer"/> and <see cref="PredefinedOrForwardingMetadataResolver"/>
        /// </summary>
        /// <seealso cref="Diagnostics.RequiredReferencesDirectiveAnalyzer.RequiredReferences"/>
        /// <seealso cref="PredefinedOrForwardingMetadataResolver.WhitelistedAssemblies"/>
        public const string TexartReferenceFileName = "Texart.Api.dll";
        /// <summary>
        /// The SkiaSharp assembly file name. For types like <see cref="SkiaSharp.SKBitmap"/>.
        /// Used by <see cref="Diagnostics.RequiredReferencesDirectiveAnalyzer"/> and <see cref="PredefinedOrForwardingMetadataResolver"/>
        /// </summary>
        /// <seealso cref="Diagnostics.RequiredReferencesDirectiveAnalyzer.RequiredReferences"/>
        /// <seealso cref="PredefinedOrForwardingMetadataResolver.WhitelistedAssemblies"/>
        public const string SkiaSharpReferenceFileName = "SkiaSharp.dll";
        /// <summary>
        /// The Texart Api assembly file name. For types like <see cref="Newtonsoft.Json.JsonConvert"/>.
        /// Used by <see cref="Diagnostics.RequiredReferencesDirectiveAnalyzer"/> and <see cref="PredefinedOrForwardingMetadataResolver"/>
        /// </summary>
        /// <seealso cref="Diagnostics.RequiredReferencesDirectiveAnalyzer.RequiredReferences"/>
        /// <seealso cref="PredefinedOrForwardingMetadataResolver.WhitelistedAssemblies"/>
        public const string NewtonsoftJsonReferenceFileName = "Newtonsoft.Json.dll";

        /// <summary>
        /// The suffix for Texart script files.
        /// </summary>
        public const string TexartScriptFileSuffix = ".tx.csx";
    }
}
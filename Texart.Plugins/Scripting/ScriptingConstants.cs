using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Texart.Plugins.Tests")]
namespace Texart.Plugins.Scripting
{
    internal static class ScriptingConstants
    {
        public const string TexartReferenceFileName = "Texart.Api.dll";
        public const string SkiaSharpReferenceFileName = "SkiaSharp.dll";
        public const string NewtonsoftJsonReferenceFileName = "Newtonsoft.Json.dll";

        public const string TexartMainFileSuffix = ".tx.csx";
    }
}
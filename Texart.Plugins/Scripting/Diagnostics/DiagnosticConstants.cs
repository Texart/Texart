using System.Dynamic;
using Microsoft.CodeAnalysis;

namespace Texart.Plugins.Scripting.Diagnostics
{
    /// <summary>
    /// Helper class containing constants for <see cref="DiagnosticDescriptor"/>.
    /// </summary>
    internal static class DiagnosticConstants
    {
        /// <summary>
        /// The commonly shared <see cref="DiagnosticDescriptor.Category"/>.
        /// </summary>
        public const string CommonCategory = "Texart.Plugins";

        /// <summary>
        /// The prefix for all <see cref="DiagnosticDescriptor.Id"/>.
        /// </summary>
        public const string TexartDiagnosticIdPrefix = "TX";

        /// <summary>
        /// The <see cref="DiagnosticDescriptor.Id"/> for <see cref="RequiredReferencesDirectiveAnalyzer"/>.
        /// </summary>
        public const string TexartReferenceDirectiveAnalyzerId = "TX1001";
    }
}
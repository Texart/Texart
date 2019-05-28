using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Texart.Plugins.Scripting.Diagnostics
{
    /// <summary>
    /// A <see cref="DiagnosticAnalyzer"/> for C# that ensures that all the required assembly references are present.
    /// This diagnostic is only available for <c>.tx.csx</c> files.
    /// </summary>
    /// <seealso cref="RequiredReferencesDirectiveAnalyzer.RequiredReferences"/>
    /// <seealso cref="ScriptingConstants.TexartScriptFileSuffix"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RequiredReferencesDirectiveAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The file names that MUST be <c>#r</c>'d at the top of Texart script files.
        /// </summary>
        /// <seealso cref="ScriptingConstants.TexartScriptFileSuffix"/>
        public static readonly ImmutableArray<string> RequiredReferences = ImmutableArray.Create(
            ScriptingConstants.TexartReferenceFileName,
            ScriptingConstants.SkiaSharpReferenceFileName,
            ScriptingConstants.NewtonsoftJsonReferenceFileName);

        /// <summary>
        /// See <see cref="DiagnosticDescriptor.Title"/>.
        /// </summary>
        private const string Title = "Texart scripts must begin with pre-determined reference directives (\"#r\")";
        /// <summary>
        /// See <see cref="DiagnosticDescriptor.MessageFormat"/>.
        /// </summary>
        private const string ScriptMustReferenceFormat = "Texart scripts must reference {0} before other code. Add \'#r \"{0}\"' at the top of the file.";
        /// <summary>
        /// See <see cref="DiagnosticDescriptor.MessageFormat"/>.
        /// </summary>
        private const string LoadDirectiveNotAllowedBeforeFormat = "#load directive is not allowed before all required references";
        /// <summary>
        /// See <see cref="DiagnosticDescriptor.Description"/>.
        /// </summary>
        private static readonly string Description =
            "Texart script must begin with the following directives:\n" +
            string.Join('\n', FormatReferences(RequiredReferences));
        /// <summary>
        /// See <see cref="DiagnosticDescriptor.DefaultSeverity"/>.
        /// </summary>
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Error;
        /// <summary>
        /// The diagnostic rule description.
        /// </summary>
        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
            title: Title,
            messageFormat: "{0}",
            category: DiagnosticConstants.CommonCategory,
            defaultSeverity: Severity,
            isEnabledByDefault: true,
            description: Description);

        /// <inheritdoc cref="DiagnosticAnalyzer.SupportedDiagnostics"/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        /// <inheritdoc cref="DiagnosticAnalyzer.Initialize"/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeRequiredReferences, SyntaxKind.CompilationUnit);
        }

        /// <summary>
        /// Checks script files for <see cref="RequiredReferences"/>.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        private static void AnalyzeRequiredReferences(SyntaxNodeAnalysisContext context)
        {
            Debug.Assert(context.Node.IsKind(SyntaxKind.CompilationUnit));
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            if (!IsTexartScript(compilationUnit.SyntaxTree))
            {
                // The references are only required in the main/executing script
                return;
            }

            var triviaList = compilationUnit.GetLeadingTrivia();
            var remainingReferences = new SortedSet<string>(RequiredReferences);
            for (
                var currentTriviaIndex = 0;
                remainingReferences.Any() && currentTriviaIndex < triviaList.Count;
                ++currentTriviaIndex)
            {
                var triviaSyntax = triviaList[currentTriviaIndex];
                if (!triviaSyntax.IsDirective)
                {
                    continue;
                }

                if (triviaSyntax.IsKind(SyntaxKind.LoadDirectiveTrivia))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Descriptor, triviaSyntax.GetLocation(),
                        LoadDirectiveNotAllowedBeforeFormat));
                    continue;
                }

                Debug.Assert(triviaSyntax.HasStructure);
                var directive = (DirectiveTriviaSyntax)triviaSyntax.GetStructure();
                if (!directive.IsKind(SyntaxKind.ReferenceDirectiveTrivia))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Descriptor, directive.GetLocation(),
                        string.Format(ScriptMustReferenceFormat, remainingReferences.First())));
                    continue;
                }

                var referenceDirective = (ReferenceDirectiveTriviaSyntax)directive;
                // Checking RequiredReferences allows duplicate references
                if (!RequiredReferences.Contains(referenceDirective.File.ValueText))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Descriptor, referenceDirective.File.GetLocation(),
                        string.Format(ScriptMustReferenceFormat, remainingReferences.First())));
                    continue;
                }

                remainingReferences.Remove(referenceDirective.File.ValueText);
            }

            foreach (var reference in remainingReferences)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Descriptor, compilationUnit.GetLocation(),
                    string.Format(ScriptMustReferenceFormat, reference)));
            }
        }

        /// <summary>
        /// Determines if <paramref name="syntaxTree"/> is from a <c>.tx.csx</c> file.
        /// </summary>
        /// <param name="syntaxTree">The parsed syntax tree to check.</param>
        /// <returns>
        ///     <c>true</c> if the syntax tree is from a texart script,
        ///     <c>false</c> otherwise.
        /// </returns>
        private static bool IsTexartScript(SyntaxTree syntaxTree)
        {
            var filePath = syntaxTree.FilePath;
            return filePath != null && filePath.EndsWith(ScriptingConstants.TexartScriptFileSuffix);
        }

        /// <summary>
        /// Format a reference as a C# directive.
        /// </summary>
        /// <param name="reference">The reference file name.</param>
        /// <returns>The formatted directive code.</returns>
        private static string FormatReference(string reference) => $"#r \"{reference}\"";

        /// <summary>
        /// Format multiple references as a bullet list of C# directives.
        /// </summary>
        /// <param name="references">The reference file names.</param>
        /// <returns>The formatted directive code.</returns>
        /// <seealso cref="FormatReference"/>
        private static IEnumerable<string> FormatReferences(IEnumerable<string> references) =>
            references.Select(reference => $"* {FormatReference(reference)}");
    }
}
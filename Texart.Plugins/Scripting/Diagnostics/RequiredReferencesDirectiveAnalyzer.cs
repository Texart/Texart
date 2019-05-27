using System;
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
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RequiredReferencesDirectiveAnalyzer : DiagnosticAnalyzer
    {
        public static readonly ImmutableArray<string> RequiredReferences = ImmutableArray.Create(
            ScriptingConstants.TexartReferenceFileName,
            ScriptingConstants.SkiaSharpReferenceFileName,
            ScriptingConstants.NewtonsoftJsonReferenceFileName);

        private const string Title = "Texart scripts must begin with pre-determined reference directives (\"#r\")";
        private const string ScriptMustReferenceFormat = "Texart scripts must reference {0} before other code. Add \'#r \"{0}\"' at the top of the file.";
        private const string LoadDirectiveNotAllowedBeforeFormat = "#load directive is not allowed before all required references";

        private static readonly string Description =
            "Texart script must begin with the following directives:\n" +
            string.Join('\n', FormatReferences(RequiredReferences));

        private const DiagnosticSeverity Severity = DiagnosticSeverity.Error;

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
            title: Title,
            messageFormat: "{0}",
            category: DiagnosticConstants.CommonCategory,
            defaultSeverity: Severity,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.CompilationUnit);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            Debug.Assert(context.Node.IsKind(SyntaxKind.CompilationUnit));
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            if (!IsExecutableScript(compilationUnit.SyntaxTree))
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

        private static bool IsExecutableScript(SyntaxTree syntaxTree)
        {
            var filePath = syntaxTree.FilePath;
            return filePath != null && filePath.EndsWith(ScriptingConstants.TexartMainFileSuffix);
        }

        private static string FormatReference(string reference) => $"#r \"{reference}\"";

        private static IEnumerable<string> FormatReferences(IEnumerable<string> references) =>
            references.Select(reference => $"* {FormatReference(reference)}");
    }
}
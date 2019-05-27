using System;
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
    public class TexartReferenceDirectiveAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string Message = $"Texart scripts must begin with \"#r \"{ScriptingConstants.TexartReferenceFileName}\"\"";

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
            title: Message,
            messageFormat: Message,
            category: DiagnosticConstants.CommonCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault:true,
            description: Message);

        private readonly string _executingScriptFilePath;

        public TexartReferenceDirectiveAnalyzer(string executingScriptFilePath)
        {
            this._executingScriptFilePath = executingScriptFilePath ??
                                            throw new ArgumentNullException(nameof(executingScriptFilePath));
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.CompilationUnit);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            Debug.Assert(context.Node.IsKind(SyntaxKind.CompilationUnit));
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            if (!IsExecutingScript(compilationUnit.SyntaxTree))
            {
                // The reference is only required in the main/executing script
                return;
            }

            var triviaList = compilationUnit.GetLeadingTrivia().SkipWhile(trivia => !trivia.IsDirective).Take(1).ToArray();
            if (!triviaList.Any())
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, compilationUnit.GetLocation()));
                return;
            }

            var firstTrivia = triviaList.First();
            Debug.Assert(firstTrivia.IsDirective);
            Debug.Assert(firstTrivia.HasStructure);
            var firstDirective = (DirectiveTriviaSyntax) firstTrivia.GetStructure();

            if (!firstDirective.IsKind(SyntaxKind.ReferenceDirectiveTrivia))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstDirective.GetLocation()));
                return;
            }
            var firstReference = (ReferenceDirectiveTriviaSyntax)firstDirective;
            if (firstReference.File.ValueText != ScriptingConstants.TexartReferenceFileName)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstReference.File.GetLocation()));
                return;
            }
        }

        private bool IsExecutingScript(SyntaxTree syntaxTree)
        {
            var filePath = syntaxTree.FilePath;
            if (filePath == null)
            {
                return false;
            }
            // We actually pass in the file path via ScriptOptions. So string equality should be fine.
            return filePath == _executingScriptFilePath;
        }
    }
}
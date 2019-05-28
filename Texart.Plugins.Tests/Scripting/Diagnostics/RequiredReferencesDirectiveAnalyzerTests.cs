using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using Texart.Plugins.Scripting;
using Texart.Plugins.Tests.Scripting.Diagnostics.TestHelpers;
using Texart.Plugins.Scripting.Diagnostics;

namespace Texart.Plugins.Tests.Scripting.Diagnostics
{
    internal class RequiredReferencesDirectiveAnalyzerTests : DiagnosticVerifier
    {
        private static ImmutableArray<string> RequiredReferences => ImmutableArray.Create(
            "B.dll", "A.dll", "C.dll");
        protected override DiagnosticAnalyzer CSharpDiagnosticAnalyzer =>
            new RequiredReferencesDirectiveAnalyzer(RequiredReferences, string.Empty);
        protected override LanguageVersion CSharpLanguageVersion => PluginScript.DefaultLanguageVersion;
        protected override SourceCodeKind CSharpSourceCodeKind => SourceCodeKind.Script;

        [Test]
        public void AllowsAllReferences()
        {
            const string code = @"
#r ""A.dll""
#r ""B.dll""
#r ""C.dll""";
            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void AllowsDuplicateReferences()
        {
            const string code = @"
#r ""A.dll""
#r ""B.dll""
#r ""C.dll""
#r ""A.dll""
#r ""B.dll""";
            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void RejectsDisjointReferences()
        {
            const string code = @"
#r ""A.dll""
#r ""C.dll""
#r ""LOL.dll""
#r ""B.dll""";
            var expected = new DiagnosticResult
            {
                Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                Message = string.Format(
                    RequiredReferencesDirectiveAnalyzer.ScriptMustReferenceFormat, "B.dll"),
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation(string.Empty, 4, 4) }
            };
            VerifyCSharpDiagnostic(code, expected);
        }

        [Test]
        public void RejectsLoadDirectiveInBetween()
        {
            const string code = @"
#r ""A.dll""
#load ""helper.csx""
#r ""C.dll""
#r ""B.dll""";
            var expected = new DiagnosticResult
            {
                Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                Message = RequiredReferencesDirectiveAnalyzer.LoadDirectiveNotAllowedBeforeFormat,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation(string.Empty, 3, 1) }
            };
            VerifyCSharpDiagnostic(code, expected);
        }

        [Test]
        public void RejectsLoadDirectiveInBetweenButStillOutputsMissingReferences()
        {
            const string code = @"
#r ""A.dll""
#load ""helper.csx""
#r ""B.dll""";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = RequiredReferencesDirectiveAnalyzer.LoadDirectiveNotAllowedBeforeFormat,
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation(string.Empty, 3, 1) }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferencesDirectiveAnalyzer.ScriptMustReferenceFormat, "C.dll"),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation(string.Empty, 4, 11) }
                },
            };
            VerifyCSharpDiagnostic(code, expected);
        }

        [Test]
        public void RejectsOneMissingReference()
        {
            const string code = @"
#r ""A.dll""
#r ""B.dll""";
            var expected = new DiagnosticResult
            {
                Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                Message = string.Format(
                    RequiredReferencesDirectiveAnalyzer.ScriptMustReferenceFormat, "C.dll"),
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation(string.Empty, 3, 11) }
            };
            VerifyCSharpDiagnostic(code, expected);
        }

        [Test]
        public void RejectsSyntaxBeforeReferences()
        {
            const string code = @"
using System;
#r ""A.dll""
#r ""B.dll""
#r ""C.dll""";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferencesDirectiveAnalyzer.ScriptMustReferenceFormat, "A.dll"),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation(string.Empty, 2, 11) }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferencesDirectiveAnalyzer.ScriptMustReferenceFormat, "B.dll"),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation(string.Empty, 2, 11) }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferencesDirectiveAnalyzer.ScriptMustReferenceFormat, "C.dll"),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation(string.Empty, 2, 11) }
                },
            };
            VerifyCSharpDiagnostic(code, expected);
        }
    }
}
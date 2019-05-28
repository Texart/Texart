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
    internal class RequiredReferenceDirectivesAnalyzerTests : DiagnosticVerifier
    {
        private static ImmutableArray<string> RequiredReferences => ImmutableArray.Create(
            "B.dll", "A.dll", "C.dll");
        protected override DiagnosticAnalyzer CSharpDiagnosticAnalyzer =>
            new RequiredReferenceDirectivesAnalyzer(RequiredReferences, string.Empty);
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
        public void AllowsRequiredReferencesLaterInCode()
        {
            const string code = @"
#r ""A.dll""
#r ""B.dll""
#r ""C.dll""
// namespace Foo
namespace Foo {}
#r ""A.dll""";
            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void AllowsOtherReferences()
        {
            const string code = @"
#r ""A.dll""
#r ""B.dll""
#r ""C.dll""
#r ""Stuff.dll""
namespace Foo {}
#r ""Stuff.dll""";
            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void AllowsPrecedingComments()
        {
            const string code = @"
// sample comment
#r ""A.dll""
#r ""B.dll""
#r ""C.dll""
namespace Foo {}";
            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void AllowsCommentsAndWhitespaceInBetween()
        {
            const string code = @"

// sample comment

#r ""A.dll""
#r ""B.dll""

// sample comment 2

#r ""C.dll""
namespace Foo {}";
            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void AllowsRegionAroundReferences()
        {
            const string code = @"
#region Region
#r ""A.dll""
#r ""B.dll""
#r ""C.dll""
#endregion";
            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void AllowsRegionBetweenReferences()
        {
            const string code = @"
#region Region
#r ""A.dll""
#r ""B.dll""
#endregion
#r ""C.dll""";
            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void AllowsPragmaBetweenReferences()
        {
            const string code = @"
#r ""A.dll""
#r ""B.dll""
#pragma warning disable 414
#r ""C.dll""";
            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void RejectsIfDirective()
        {
            const string code = @"
#if DEBUG
#r ""A.dll""
#else
#r ""B.dll""
#endif
#r ""C.dll""";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferenceDirectivesAnalyzer.DirectiveNotAllowedFormat, "if"),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] {new DiagnosticResultLocation(string.Empty, 2, 1)}
                },
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferenceDirectivesAnalyzer.ScriptMustReferenceFormat, @"""A.dll"", ""B.dll"", ""C.dll"""),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] {new DiagnosticResultLocation(string.Empty, 2, 1)}
                },
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferenceDirectivesAnalyzer.DirectiveNotAllowedFormat, "else"),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] {new DiagnosticResultLocation(string.Empty, 4, 1)}
                },
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferenceDirectivesAnalyzer.ScriptMustReferenceFormat, @"""B.dll"", ""C.dll"""),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] {new DiagnosticResultLocation(string.Empty, 4, 1)}
                },
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferenceDirectivesAnalyzer.DirectiveNotAllowedFormat, "endif"),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] {new DiagnosticResultLocation(string.Empty, 6, 1)}
                },
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferenceDirectivesAnalyzer.ScriptMustReferenceFormat, @"""C.dll"""),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] {new DiagnosticResultLocation(string.Empty, 6, 1)}
                },
            };
            VerifyCSharpDiagnostic(code, expected);
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
                    RequiredReferenceDirectivesAnalyzer.ScriptMustReferenceFormat, @"""B.dll"""),
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
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferenceDirectivesAnalyzer.DirectiveNotAllowedFormat, "load"),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation(string.Empty, 3, 1) }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferenceDirectivesAnalyzer.ScriptMustReferenceFormat, @"""B.dll"", ""C.dll"""),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation(string.Empty, 3, 1) }
                },
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
                    Message = string.Format(
                        RequiredReferenceDirectivesAnalyzer.DirectiveNotAllowedFormat, "load"),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation(string.Empty, 3, 1) }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferenceDirectivesAnalyzer.ScriptMustReferenceFormat, @"""B.dll"", ""C.dll"""),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation(string.Empty, 3, 1) }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferenceDirectivesAnalyzer.ScriptMustReferenceFormat, @"""C.dll"""),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation(string.Empty, 4, 11) }
                },
            };
            VerifyCSharpDiagnostic(code, expected);
        }

        [Test]
        public void RejectsUnknownDirective()
        {
            const string code = @"
#r ""A.dll""
#r ""B.dll""
#unknown
#r ""C.dll""";
            var expected = new DiagnosticResult
            {
                Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                Message = string.Format(
                    RequiredReferenceDirectivesAnalyzer.DirectiveNotRecognizedFormat, "unknown", @"""C.dll"""),
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {new DiagnosticResultLocation(string.Empty, 4, 1)}
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
                    RequiredReferenceDirectivesAnalyzer.ScriptMustReferenceFormat, @"""C.dll"""),
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
                        RequiredReferenceDirectivesAnalyzer.ScriptMustReferenceFormat, @"""A.dll"""),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation(string.Empty, 2, 11) }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferenceDirectivesAnalyzer.ScriptMustReferenceFormat, @"""B.dll"""),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation(string.Empty, 2, 11) }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticConstants.TexartReferenceDirectiveAnalyzerId,
                    Message = string.Format(
                        RequiredReferenceDirectivesAnalyzer.ScriptMustReferenceFormat, @"""C.dll"""),
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation(string.Empty, 2, 11) }
                },
            };
            VerifyCSharpDiagnostic(code, expected);
        }
    }

    internal class RequireReferenceDirectivesAnalyzerSkipFilesTests : DiagnosticVerifier
    {
        // A crazy extension for scripts to make sure that any input file gets ignored by the analyzer
        protected override DiagnosticAnalyzer CSharpDiagnosticAnalyzer =>
            new RequiredReferenceDirectivesAnalyzer(
                RequiredReferenceDirectivesAnalyzer.RequiredReferences, ".crazy-extension");
        protected override LanguageVersion CSharpLanguageVersion => PluginScript.DefaultLanguageVersion;
        protected override SourceCodeKind CSharpSourceCodeKind => SourceCodeKind.Script;

        [Test]
        public void IgnoresNonScriptFile()
        {
            const string code = @"
#load ""foo.csx""
namespace Foo
{
    class Program { }
}";
            VerifyCSharpDiagnostic(code);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Texart.Plugins.Internal;

namespace Texart.Plugins.Scripting
{
    public class TranslationUnit
    {
        private static LanguageVersion DefaultLanguageVersion => LanguageVersion.CSharp8;
        private static OptimizationLevel DefaultOptimizationLevel =>
            CompilationDefines.IsRelease ? OptimizationLevel.Release : OptimizationLevel.Debug;
        private static bool DefaultEmitDebugInformation => CompilationDefines.IsDebug;
        private static bool DefaultAllowUnsafe => false;
        private static bool DefaultCheckOverflow => false;
        private static int DefaultWarningLevel => 4;
        private static Encoding DefaultFileEncoding => Encoding.UTF8;

        private static ScriptOptions DefaultScriptOptions => ScriptOptions.Default
            .WithLanguageVersion(DefaultLanguageVersion)
            .WithOptimizationLevel(DefaultOptimizationLevel)
            .WithEmitDebugInformation(DefaultEmitDebugInformation)
            .WithAllowUnsafe(DefaultAllowUnsafe)
            .WithCheckOverflow(DefaultCheckOverflow)
            .WithWarningLevel(DefaultWarningLevel)
            .WithFileEncoding(DefaultFileEncoding);

        private readonly Script<int> _script;

        public TranslationUnit(SourceFile sourceFile)
        {
            if (sourceFile == null) throw new ArgumentNullException(nameof(sourceFile));
            var scriptOptions = DefaultScriptOptions
                .WithFilePath(sourceFile.FilePath)
                .WithSourceResolver(BuildReferenceResolverForFile(sourceFile));
            _script = CSharpScript.Create<int>(sourceFile.Code, scriptOptions);
        }

        public async Task<int> RunAsync()
        {
            return (await _script.RunAsync()).ReturnValue;
        }

        private static SourceReferenceScheme FileReferenceScheme => new SourceReferenceScheme("file");

        private static SourceReferenceResolver BuildReferenceResolverForFile(SourceFile sourceFile)
        {
            var fileResolver = new SourceFileResolver(ImmutableArray<string>.Empty, Path.GetDirectoryName(sourceFile.FilePath));
            var resolvers = new Dictionary<SourceReferenceScheme, SourceReferenceResolver>
            {
                {FileReferenceScheme, fileResolver}
            };
            return new SwitchedSourceReferenceResolver(fileResolver, resolvers.ToImmutableDictionary());
        }
    }
}

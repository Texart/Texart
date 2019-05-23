using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Texart.Api;
using Texart.Plugins.Internal;

namespace Texart.Plugins.Scripting
{
    public static class PluginScript
    {
        public static Script<IPlugin> From(SourceFile sourceFile)
        {
            if (sourceFile == null) throw new ArgumentNullException(nameof(sourceFile));
            var scriptOptions = DefaultScriptOptions
                .WithFilePath(sourceFile.FilePath)
                .WithSourceResolver(BuildReferenceResolverForFile(sourceFile));
            return CSharpScript.Create<IPlugin>(sourceFile.Code, scriptOptions);
        }

        private static LanguageVersion DefaultLanguageVersion => LanguageVersion.CSharp8;
        private static OptimizationLevel DefaultOptimizationLevel =>
            CompilationDefines.IsRelease ? OptimizationLevel.Release : OptimizationLevel.Debug;
        private static bool DefaultEmitDebugInformation => CompilationDefines.IsDebug;
        private static bool DefaultAllowUnsafe => false;
        private static bool DefaultCheckOverflow => false;
        private static int DefaultWarningLevel => 4;
        private static Encoding DefaultFileEncoding => Encoding.UTF8;

        private static Assembly[] DefaultExtraAssemblies => new[]
        {
            // This will also bring in the transitive dependencies of Texart.Api, including:
            // * Newtonsoft.Json
            // * SkiaSharp
            // Refer to Texart.Api.csproj for complete list
            typeof(IPlugin).Assembly,
        };

        private static ScriptOptions DefaultScriptOptions => ScriptOptions.Default
            .WithLanguageVersion(DefaultLanguageVersion)
            .WithOptimizationLevel(DefaultOptimizationLevel)
            .WithEmitDebugInformation(DefaultEmitDebugInformation)
            .WithAllowUnsafe(DefaultAllowUnsafe)
            .WithCheckOverflow(DefaultCheckOverflow)
            .WithWarningLevel(DefaultWarningLevel)
            .WithFileEncoding(DefaultFileEncoding)
            .AddReferences(DefaultExtraAssemblies);

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
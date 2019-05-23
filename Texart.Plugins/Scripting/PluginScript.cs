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
    /// <summary>
    /// Helper methods for creating <see cref="Script{T}"/> instances with the correct options for Texart.
    /// </summary>
    public static class PluginScript
    {
        /// <summary>
        /// Creates a <code>Script</code> instance that will execute the provided <code>SourceFile</code>.
        /// The returned <code>Script</code> is pre-configured with Texart-specific compiler options.
        /// </summary>
        /// <param name="sourceFile">the source file to run</param>
        /// <returns>Script instance</returns>
        public static Script<IPlugin> From(SourceFile sourceFile)
        {
            if (sourceFile == null) throw new ArgumentNullException(nameof(sourceFile));
            var scriptOptions = BaseScriptOptions
                .WithFilePath(sourceFile.FilePath)
                .WithSourceResolver(BuildReferenceResolverForFile(sourceFile));
            return CSharpScript.Create<IPlugin>(sourceFile.Code, scriptOptions);
        }

        /// <summary>
        /// Creates a <code>Script</code> instance that will execute a <code>SourceFile</code> loaded from
        /// the provided path.
        /// The returned <code>Script</code> is pre-configured with Texart-specific compiler options.
        /// </summary>
        /// <param name="sourceFilePath">The file to load source file from.</param>
        /// <returns>Script instance</returns>
        /// <see cref="From(SourceFile)"/>
        /// <see cref="SourceFile"/>
        public static Script<IPlugin> LoadFrom(string sourceFilePath) => From(SourceFile.Load(sourceFilePath));

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

        /// <summary>
        /// Pre-configured default script options.
        /// </summary>
        private static ScriptOptions BaseScriptOptions => ScriptOptions.Default
            .WithLanguageVersion(DefaultLanguageVersion)
            .WithOptimizationLevel(DefaultOptimizationLevel)
            .WithEmitDebugInformation(DefaultEmitDebugInformation)
            .WithAllowUnsafe(DefaultAllowUnsafe)
            .WithCheckOverflow(DefaultCheckOverflow)
            .WithWarningLevel(DefaultWarningLevel)
            .WithFileEncoding(DefaultFileEncoding)
            .AddReferences(DefaultExtraAssemblies);

        private static ReferenceScheme FileReferenceScheme => new ReferenceScheme("file");

        /// <summary>
        /// Creates a <code>SourceReferenceResolver</code> that is able to recognize different schemes are forward to
        /// appropriate resolvers. <see cref="SourceReferenceResolverDemux"/>.
        /// </summary>
        /// <param name="sourceFile">The C# script</param>
        /// <returns>A custom resolver</returns>
        private static SourceReferenceResolver BuildReferenceResolverForFile(SourceFile sourceFile)
        {
            var fileResolver = new SourceFileResolver(ImmutableArray<string>.Empty, Path.GetDirectoryName(sourceFile.FilePath));
            var resolvers = new Dictionary<ReferenceScheme, SourceReferenceResolver>
            {
                {FileReferenceScheme, fileResolver}
            };
            return new SourceReferenceResolverDemux(fileResolver, resolvers.ToImmutableDictionary());
        }
    }
}
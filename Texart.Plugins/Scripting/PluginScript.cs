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
    /// Helper methods for creating <see cref="Script{T}"/> instances, flavored for Texart.
    /// </summary>
    public static class PluginScript
    {
        /// <summary>
        /// Creates a <see cref="Script"/> instance that will execute the provided <c>SourceFile</c>.
        /// The returned <see cref="Script"/> is pre-configured with Texart-specific compiler options.
        /// </summary>
        /// <param name="sourceFile">The source file to run.</param>
        /// <returns>Script instance.</returns>
        public static Script<IPlugin> From(SourceFile sourceFile) => From<IPlugin>(sourceFile);

        /// <summary>
        /// Creates a <see cref="Script"/> instance that will execute the provided <c>SourceFile</c>.
        /// The returned <see cref="Script"/> is pre-configured with Texart-specific compiler options.
        /// </summary>
        /// <typeparam name="T">The return type of the script.</typeparam>
        /// <param name="sourceFile">The source file to run.</param>
        /// <returns>Script instance</returns>
        public static Script<T> From<T>(SourceFile sourceFile)
        {
            if (sourceFile == null) throw new ArgumentNullException(nameof(sourceFile));
            var scriptOptions = BaseScriptOptions
                .WithFilePath(sourceFile.FilePath)
                .WithSourceResolver(BuildSourceReferenceResolver(sourceFile))
                .WithMetadataResolver(BuildMetadataReferenceResolver(sourceFile));
            return CSharpScript.Create<T>(sourceFile.Text, scriptOptions);
        }

        /// <summary>
        /// Creates a <see cref="Script"/> instance that will execute a <c>SourceFile</c> loaded from
        /// the provided path.
        /// The returned <see cref="Script"/> is pre-configured with Texart-specific compiler options.
        /// </summary>
        /// <typeparam name="T">The return type of the script.</typeparam>
        /// <param name="sourceFilePath">The file to load source file from.</param>
        /// <returns>Script instance.</returns>
        /// <see cref="From(SourceFile)"/>
        /// <see cref="SourceFile"/>
        public static Script<T> LoadFrom<T>(string sourceFilePath) => From<T>(SourceFile.Load(sourceFilePath));

        /// <summary>
        /// Creates a <see cref="Script"/> instance that will execute a <c>SourceFile</c> loaded from
        /// the provided path.
        /// The returned <see cref="Script"/> is pre-configured with Texart-specific compiler options.
        /// </summary>
        /// <param name="sourceFilePath">The file to load source file from.</param>
        /// <returns>Script instance.</returns>
        /// <see cref="From(SourceFile)"/>
        /// <see cref="SourceFile"/>
        public static Script<IPlugin> LoadFrom(string sourceFilePath) => LoadFrom<IPlugin>(sourceFilePath);

        private static LanguageVersion DefaultLanguageVersion => LanguageVersion.CSharp7_3;
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
        /// Creates a <see cref="SourceReferenceResolver"/> that is able to recognize different schemes are forward to
        /// appropriate resolvers. <see cref="SourceReferenceResolverDemux"/>.
        /// </summary>
        /// <param name="sourceFile">The C# script.</param>
        /// <returns>A custom resolver.</returns>
        private static SourceReferenceResolver BuildSourceReferenceResolver(SourceFile sourceFile)
        {
            var fileResolver = new SourceFileResolver(ImmutableArray<string>.Empty, Path.GetDirectoryName(sourceFile.FilePath));
            var resolvers = new Dictionary<ReferenceScheme, SourceReferenceResolver>
            {
                {FileReferenceScheme, fileResolver}
            };
            return new SourceReferenceResolverDemux(fileResolver, resolvers.ToImmutableDictionary());
        }

        /// <summary>
        /// Creates a <see cref="MetadataReferenceResolver"/see> that is able to recognize different schemes are forward to
        /// appropriate resolvers. <see cref="MetadataReferenceResolverDemux"/>.
        /// </summary>
        /// <param name="sourceFile">The C# script.</param>
        /// <returns>A custom resolver.</returns>
        private static MetadataReferenceResolver BuildMetadataReferenceResolver(SourceFile sourceFile)
        {
            var scriptResolver = ScriptMetadataResolver.Default.WithBaseDirectory(Path.GetDirectoryName(sourceFile.FilePath));
            var resolvers = new Dictionary<ReferenceScheme, MetadataReferenceResolver>
            {
                {FileReferenceScheme, scriptResolver}
            };
            return new MetadataReferenceResolverDemux(scriptResolver, resolvers.ToImmutableDictionary());
        }
    }
}
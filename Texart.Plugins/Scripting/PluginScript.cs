﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Scripting;
using Texart.Api;
using Texart.Plugins.Scripting.Diagnostics;

namespace Texart.Plugins.Scripting
{
    /// <summary>
    /// Wrapper around <see cref="Script{T}"/>, flavored for Texart.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class PluginScript<T>
    {
        /// <summary>
        /// The pre-configured <see cref="Microsoft.CodeAnalysis.Scripting.Script"/> instance.
        /// </summary>
        private Script<T> Script { get; }

        /// <summary>
        /// Constructs a <see cref="PluginScript{T}"/> with the provided <see cref="Script{T}"/> instance.
        /// </summary>
        /// <param name="script">The backing <see cref="Script{T}"/> instance.</param>
        internal PluginScript(Script<T> script)
        {
            Debug.Assert(script != null);
            Script = script;
        }

        /// <summary>
        /// Custom Roslyn analyzers for Texart.
        /// </summary>
        private static ImmutableArray<DiagnosticAnalyzer> CustomAnalyzers => ImmutableArray.Create<DiagnosticAnalyzer>(
            new RequiredReferenceDirectivesAnalyzer());

        /// <summary>
        /// Asynchronously compile the script and return any diagnostics.
        /// </summary>
        /// @todo Figure out if custom diagnostics can be done in the same pass as the default ones
        /// <returns>A compiled <see cref="Script{T}"/> object with compile diagnostics.</returns>
        public async Task<PluginScriptCompilation<T>> Compile()
        {
            // TODO: figure out if Task.Run is actually needed in these cases
            var compilation = await Task.Run(() => Script.GetCompilation());
            var customCompilation = compilation.WithAnalyzers(CustomAnalyzers);
            var customDiagnostics = (await customCompilation.GetAllDiagnosticsAsync())
                .Where(IsCustomDiagnostic)
                .ToImmutableArray();
            var defaultDiagnostics = await Task.Run(() => Script.Compile());
            return new PluginScriptCompilation<T>(Script, customDiagnostics, defaultDiagnostics);
        }

        /// <summary>
        /// Determines if <paramref name="diagnostic"/> is one defined in Texart.
        /// </summary>
        /// <param name="diagnostic">The <see cref="Diagnostic"/> to check.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="diagnostic"/> is defined in Texart,
        ///     <c>false</c> otherwise.
        /// </returns>
        private static bool IsCustomDiagnostic(Diagnostic diagnostic) =>
            diagnostic.Id.StartsWith(DiagnosticConstants.TexartDiagnosticIdPrefix);
    }

    /// <summary>
    /// The result of a <see cref="PluginScript{T}"/> compilation.
    /// </summary>
    /// <typeparam name="T">The return type of the underling <see cref="Script{T}"/></typeparam>
    /// <seealso cref="PluginScript{T}.Compile"/>
    public sealed class PluginScriptCompilation<T>
    {
        /// <summary>
        /// The diagnostics of compiling <see cref="Script"/>.
        /// </summary>
        public ImmutableArray<Diagnostic> Diagnostics => CustomDiagnostics.AddRange(DefaultDiagnostics);

        /// <summary>
        /// Custom diagnostics for Texart.
        /// </summary>
        public ImmutableArray<Diagnostic> CustomDiagnostics { get; }

        /// <summary>
        /// Diagnostics directly from the compiler.
        /// </summary>
        public ImmutableArray<Diagnostic> DefaultDiagnostics { get; }

        /// <summary>
        /// A wrapper for <see cref="Script{T}.RunAsync(object,System.Threading.CancellationToken)"/> with custom diagnostics added.
        /// </summary>
        /// <param name="globals">See <see cref="Script{T}.RunAsync(object,System.Threading.CancellationToken)"/>.</param>
        /// <param name="catchException">See <see cref="Script{T}.RunAsync(object,System.Threading.CancellationToken)"/>.</param>
        /// <param name="cancellationToken">See <see cref="Script{T}.RunAsync(object,System.Threading.CancellationToken)"/>.</param>
        /// <returns>The <see cref="ScriptState{T}.ReturnValue"/>.</returns>
        /// <seealso cref="Script{T}.RunAsync(object,System.Threading.CancellationToken)"/>.
        public async Task<ScriptState<T>> RunAsync(
            object? globals = null,
            Func<Exception, bool>? catchException = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var existingDiagnostics = Diagnostics;
                if (existingDiagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error))
                {
                    // diagnostics exist from compilation, no need to call RunAsync
                    throw new CompilationErrorException(FormatDiagnosticMessage(existingDiagnostics), existingDiagnostics);
                }
                return await Script.RunAsync(globals, catchException, cancellationToken);
            }
            catch (CompilationErrorException ex)
            {
                if (CustomDiagnostics.IsEmpty)
                {
                    // without custom diagnostics, no change is necessary
                    throw;
                }
                var diagnostics = CustomDiagnostics.AddRange(ex.Diagnostics);
                throw new CompilationErrorException(FormatDiagnosticMessage(diagnostics), diagnostics);
            }
            string FormatDiagnosticMessage(IEnumerable<Diagnostic> diagnostics) =>
                PluginScript.DiagnosticFormatter.Format(diagnostics.First(), CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// A wrapper for <see cref="RunAsync"/> that returns the <see cref="ScriptState{T}.ReturnValue"/> directly.
        /// </summary>
        /// <param name="globals">See <see cref="Script{T}.RunAsync(object,System.Threading.CancellationToken)"/>.</param>
        /// <param name="catchException">See <see cref="Script{T}.RunAsync(object,System.Threading.CancellationToken)"/>.</param>
        /// <param name="cancellationToken">See <see cref="Script{T}.RunAsync(object,System.Threading.CancellationToken)"/>.</param>
        /// <returns>The <see cref="ScriptState{T}.ReturnValue"/>.</returns>
        /// <seealso cref="Script{T}.RunAsync(object,System.Threading.CancellationToken)"/>.
        public async Task<T> EvaluateAsync(
            object? globals = null,
            Func<Exception, bool>? catchException = null,
            CancellationToken cancellationToken = default)
        {
            var scriptState = await RunAsync(globals, catchException, cancellationToken);
            return scriptState.ReturnValue;
        }

        /// <summary>
        /// Constructs a <see cref="PluginScript{T}"/> with the provided <see cref="Script{T}"/> instance and
        /// the <see cref="Diagnostic"/> results that were yielded during compilation.
        /// </summary>
        /// <param name="script">The backing <see cref="Script{T}"/> instance.</param>
        /// <param name="customDiagnostics">The custom diagnostics of compiling <paramref name="script"/>.</param>
        /// <param name="defaultDiagnostics">The default compiler diagnostics of compiling <paramref name="script"/>.</param>
        internal PluginScriptCompilation(Script<T> script, ImmutableArray<Diagnostic> customDiagnostics, ImmutableArray<Diagnostic> defaultDiagnostics)
        {
            Debug.Assert(script != null);
            Debug.Assert(customDiagnostics != null);
            Debug.Assert(defaultDiagnostics != null);
            Script = script;
            CustomDiagnostics = customDiagnostics;
            DefaultDiagnostics = defaultDiagnostics;
        }

        /// <summary>
        /// The pre-configured <see cref="Microsoft.CodeAnalysis.Scripting.Script"/> instance.
        /// </summary>
        /// <seealso cref="PluginScript{T}.Script"/>
        private Script<T> Script { get; }
    }

    /// <summary>
    /// Helper methods for creating <see cref="PluginScript{T}"/> instances.
    /// </summary>
    public static class PluginScript
    {
        /// <summary>
        /// Creates a <see cref="PluginScript"/> instance that will execute the provided <see cref="SourceFile"/>.
        /// The returned <see cref="PluginScript"/> is pre-configured with Texart-specific compiler options.
        /// </summary>
        /// <param name="sourceFile">The source file to run.</param>
        /// <returns>Script instance.</returns>
        public static PluginScript<ITxPlugin> From(SourceFile sourceFile) => From<ITxPlugin>(sourceFile);

        /// <summary>
        /// Creates a <see cref="PluginScript"/> instance that will execute the provided <see cref="SourceFile"/>.
        /// The returned <see cref="PluginScript"/> is pre-configured with Texart-specific compiler options.
        /// </summary>
        /// <typeparam name="T">The return type of the script.</typeparam>
        /// <param name="sourceFile">The source file to run.</param>
        /// <returns>Script instance</returns>
        public static PluginScript<T> From<T>(SourceFile sourceFile)
        {
            if (sourceFile == null) throw new ArgumentNullException(nameof(sourceFile));
            var scriptOptions = BaseScriptOptions
                .WithFilePath(sourceFile.FilePath)
                .WithSourceResolver(BuildSourceReferenceResolver(sourceFile))
                .WithMetadataResolver(BuildMetadataReferenceResolver(sourceFile));
            return new PluginScript<T>(CSharpScript.Create<T>(sourceFile.Text, scriptOptions));
        }

        /// <summary>
        /// Creates a <see cref="PluginScript"/> instance that will execute a <see cref="SourceFile"/> loaded from
        /// the provided path.
        /// The returned <see cref="PluginScript"/> is pre-configured with Texart-specific compiler options.
        /// </summary>
        /// <typeparam name="T">The return type of the script.</typeparam>
        /// <param name="sourceFilePath">The file to load source file from.</param>
        /// <returns>Script instance.</returns>
        /// <see cref="From(SourceFile)"/>
        /// <see cref="SourceFile"/>
        public static PluginScript<T> LoadFrom<T>(string sourceFilePath) => From<T>(SourceFile.Load(sourceFilePath));

        /// <summary>
        /// Creates a <see cref="PluginScript"/> instance that will execute a <see cref="SourceFile"/> loaded from
        /// the provided path.
        /// The returned <see cref="PluginScript"/> is pre-configured with Texart-specific compiler options.
        /// </summary>
        /// <param name="sourceFilePath">The file to load source file from.</param>
        /// <returns>Script instance.</returns>
        /// <see cref="From(SourceFile)"/>
        /// <see cref="SourceFile"/>
        public static PluginScript<ITxPlugin> LoadFrom(string sourceFilePath) => LoadFrom<ITxPlugin>(sourceFilePath);

        /// <summary>
        /// The language version to use to compile Texart scripts. Ideally, this should be the same as the version
        /// used to compile Texart.
        /// </summary>
        internal static LanguageVersion DefaultLanguageVersion => LanguageVersion.CSharp8;
        /// <summary>
        /// The default optimization level to compile Texart scripts.
        /// </summary>
        /// <seealso cref="ScriptOptions.OptimizationLevel"/>
        private static OptimizationLevel DefaultOptimizationLevel =>
#if DEBUG
            OptimizationLevel.Debug;
#else
            OptimizationLevel.Release;
#endif
        /// <summary>
        /// Whether or not debug info will be available in the compilation for Texart scripts.
        /// </summary>
        /// <seealso cref="ScriptOptions.EmitDebugInformation"/>
        private static bool DefaultEmitDebugInformation =>
#if DEBUG
            true;
#else
            false;
#endif
        /// <summary>
        /// Whether the <c>unsafe</c> keyword is allowed.
        /// </summary>
        /// <seealso cref="ScriptOptions.AllowUnsafe"/>
        private static bool DefaultAllowUnsafe => false;
        /// <summary>
        /// Whether overflow is checked by default (<c>checked { }</c> or <c>unchecked { }</c>).
        /// </summary>
        /// <seealso cref="ScriptOptions.CheckOverflow"/>
        private static bool DefaultCheckOverflow => false;
        /// <summary>
        /// Warning level of compiler. <c>4</c> is the max.
        /// </summary>
        /// <seealso cref="ScriptOptions.WarningLevel"/>
        private static int DefaultWarningLevel => 4;
        /// <summary>
        /// The file encoding of the Texart script source file.
        /// </summary>
        /// <seealso cref="ScriptOptions.FileEncoding"/>
        private static Encoding DefaultFileEncoding => Encoding.UTF8;
        /// <summary>
        /// Extra assemblies (and transitive dependencies) to load in Texart scripts.
        /// </summary>
        private static Assembly[] DefaultExtraAssemblies => new Assembly[]
        {
            // The Texart Api assembly is now loaded via `TexartApiScriptMetadataResolver`.
            // This is just for extra goodies now - non-essential.
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

        /// <summary>
        /// Creates a <see cref="SourceReferenceResolver"/> that is able to recognize different schemes are forward to
        /// appropriate resolvers. <see cref="SourceReferenceResolverDemux"/>.
        /// </summary>
        /// <param name="sourceFile">The C# script.</param>
        /// <returns>A custom resolver.</returns>
        private static SourceReferenceResolver BuildSourceReferenceResolver(SourceFile sourceFile)
        {
            var fileResolver = new SourceFileResolver(ImmutableArray<string>.Empty, Path.GetDirectoryName(sourceFile.FilePath));
            var resolvers = new Dictionary<TxReferenceScheme, SourceReferenceResolver>
            {
                {TxReferenceScheme.File, fileResolver}
            };
            return new SourceReferenceResolverDemux(fileResolver, resolvers.ToImmutableDictionary());
        }

        /// <summary>
        /// Creates a <see cref="MetadataReferenceResolver"/> that is able to recognize different schemes are forward to
        /// appropriate resolvers. <see cref="MetadataReferenceResolverDemux"/>.
        /// </summary>
        /// <param name="sourceFile">The C# script.</param>
        /// <returns>A custom resolver.</returns>
        private static MetadataReferenceResolver BuildMetadataReferenceResolver(SourceFile sourceFile)
        {
            var scriptResolver = ScriptMetadataResolver.Default.WithBaseDirectory(Path.GetDirectoryName(sourceFile.FilePath));
            var texartApiWrappedResolver = new PredefinedOrForwardingMetadataResolver(scriptResolver);
            var resolvers = new Dictionary<TxReferenceScheme, MetadataReferenceResolver>
            {
                {TxReferenceScheme.File, scriptResolver}
            };
            return new MetadataReferenceResolverDemux(texartApiWrappedResolver, resolvers.ToImmutableDictionary());
        }

        /// <summary>
        /// The internally shared <see cref="Microsoft.CodeAnalysis.DiagnosticFormatter"/> instance.
        /// </summary>
        internal static readonly DiagnosticFormatter DiagnosticFormatter = new DiagnosticFormatter();
    }
}
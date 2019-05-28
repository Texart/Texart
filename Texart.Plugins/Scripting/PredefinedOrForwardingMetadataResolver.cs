using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using Texart.Api;

namespace Texart.Plugins.Scripting
{
    /// <summary>
    /// A <see cref="MetadataReferenceResolver"/> that either resolves from a predefined whitelist of assemblies (<see cref="WhitelistedAssemblies"/>),
    /// or forwards the call to an underlying <see cref="MetadataReferenceResolver"/> instance.
    /// </summary>
    internal sealed class PredefinedOrForwardingMetadataResolver : MetadataReferenceResolver, IEquatable<PredefinedOrForwardingMetadataResolver>
    {
        /// <summary>
        /// Assemblies that are treated specially. These assemblies are resolved to pre-defined assemblies rather than
        /// checking the file system.
        /// </summary>
        public static readonly ImmutableDictionary<string, Assembly> WhitelistedAssemblies = new Dictionary<string, Assembly>
        {
            // The <c>Texart.Api.dll</c> assembly.
            // This will also bring in the transitive dependencies of Texart.Api, including:
            // * Newtonsoft.Json
            // * SkiaSharp
            // Refer to Texart.Api.csproj for complete list
            {ScriptingConstants.TexartReferenceFileName, typeof(IPlugin).Assembly},
            // The <c>SkiaSharp.dll</c> assembly.
            // Our public API depends Skia in several places.
            {ScriptingConstants.SkiaSharpReferenceFileName, typeof(SKBitmap).Assembly},
            // The <c>Newtonsoft.Json.dll</c> assembly.
            // Our public API also depends on JSON (for arguments to plugins)
            {ScriptingConstants.NewtonsoftJsonReferenceFileName, typeof(JToken).Assembly},
        }.ToImmutableDictionary();

        /// <summary>
        /// The underlying resolver to which we forward our calls, except for <see cref="WhitelistedAssemblies"/>.
        /// </summary>
        private readonly MetadataReferenceResolver _underlyingResolver;

        /// <summary>
        /// This was stripped from the implementation details of <see cref="Microsoft.CodeAnalysis.Scripting.ScriptMetadataResolver"/>.
        /// </summary>
        private static readonly MetadataReferenceProperties ResolvedMissingAssemblyReferenceProperties =
            MetadataReferenceProperties.Assembly.WithAliases(ImmutableArray.Create("<implicit>"));

        /// <summary>
        /// Constructs a resolver backed by the provided underlying resolver.
        /// </summary>
        /// <param name="underlyingResolver">See <see cref="_underlyingResolver"/></param>
        public PredefinedOrForwardingMetadataResolver(MetadataReferenceResolver underlyingResolver)
        {
            Debug.Assert(underlyingResolver != null);
            _underlyingResolver = underlyingResolver;
        }

        /// <inheritdoc />
        public override ImmutableArray<PortableExecutableReference> ResolveReference(
            string reference,
            string baseFilePath,
            MetadataReferenceProperties properties)
        {
            if (WhitelistedAssemblies.TryGetValue(reference, out var assembly))
            {
                return ImmutableArray.Create(
                    MetadataReference.CreateFromFile(assembly.Location, properties));
            }
            return _underlyingResolver.ResolveReference(reference, baseFilePath, properties);
        }

        /// <inheritdoc/>
        public override bool ResolveMissingAssemblies => _underlyingResolver.ResolveMissingAssemblies;

        /// <inheritdoc />
        public override PortableExecutableReference ResolveMissingAssembly(
            MetadataReference definition, AssemblyIdentity referenceIdentity)
        {
            var definitionPath = (definition as PortableExecutableReference)?.FilePath;
            if (definitionPath != null && WhitelistedAssemblies.TryGetValue(definitionPath, out var assembly))
            {
                return MetadataReference.CreateFromFile(
                    assembly.Location,
                    ResolvedMissingAssemblyReferenceProperties);
            }
            return _underlyingResolver.ResolveMissingAssembly(definition, referenceIdentity);
        }

        /// <inheritdoc />
        public bool Equals(PredefinedOrForwardingMetadataResolver other)
        {
            if (other is null) return false;
            return ReferenceEquals(this, other) || Equals(_underlyingResolver, other._underlyingResolver);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is PredefinedOrForwardingMetadataResolver other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() =>
            _underlyingResolver != null ? _underlyingResolver.GetHashCode() : 0;
    }
}
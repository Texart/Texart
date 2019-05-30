﻿using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Texart.Api;

namespace Texart.Plugins.Scripting
{
    /// <summary>
    /// A metadata reference resolver that selects one of multiple resolvers based on a matching <see cref="TxReferenceScheme"/>.
    /// In the case where there is no matching scheme, it uses the default resolver as a fallback.
    /// </summary>
    public sealed class MetadataReferenceResolverDemux : MetadataReferenceResolver, IEquatable<MetadataReferenceResolverDemux>
    {
        private readonly MetadataReferenceResolver _defaultResolver;
        private readonly ImmutableDictionary<TxReferenceScheme, MetadataReferenceResolver> _resolversByScheme;

        /// <summary>
        /// Creates an instance with a default fallback resolver, and a lookup table for different schemes.
        /// </summary>
        /// <param name="defaultResolver">The fallback resolvers.</param>
        /// <param name="resolversByScheme">The lookup table for different schemes.</param>
        public MetadataReferenceResolverDemux(MetadataReferenceResolver defaultResolver, ImmutableDictionary<TxReferenceScheme, MetadataReferenceResolver> resolversByScheme)
        {
            _defaultResolver = defaultResolver ?? throw new ArgumentNullException(nameof(defaultResolver));
            _resolversByScheme = resolversByScheme ?? throw new ArgumentNullException(nameof(resolversByScheme));
        }

        /// <inheritdoc />
        public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string baseFilePath, MetadataReferenceProperties properties)
        {
            var (scheme, resolver) = GetResolverByPath(reference);
            return resolver.ResolveReference(
                scheme != null ? scheme.NormalizePath(reference) : reference,
                baseFilePath,
                properties);
        }

        /// <inheritdoc />
        public override bool ResolveMissingAssemblies => _defaultResolver.ResolveMissingAssemblies;

        /// <inheritdoc />
        public override PortableExecutableReference ResolveMissingAssembly(MetadataReference definition, AssemblyIdentity referenceIdentity) =>
            _defaultResolver.ResolveMissingAssembly(definition, referenceIdentity);

        /// <summary>
        /// Gets the resolver by checking the provided path for a matching scheme.
        /// </summary>
        /// <param name="path">The path to check for scheme.</param>
        /// <returns>Matching resolver if found, else the default resolver.</returns>
        private (TxReferenceScheme?, MetadataReferenceResolver) GetResolverByPath(string path) =>
            _resolversByScheme
                .Where(kv => kv.Key.Matches(path))
                .Select(kv => (kv.Key, kv.Value))
                .DefaultIfEmpty((null, _defaultResolver))
                .First();

        /// <inheritdoc />
        public bool Equals(MetadataReferenceResolverDemux other) =>
            _defaultResolver.Equals(other._defaultResolver) && _resolversByScheme.Equals(other._resolversByScheme);

        /// <inheritdoc />
        public override bool Equals(object other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            switch (other)
            {
                case MetadataReferenceResolverDemux resolver when resolver.GetType() == GetType():
                    return Equals(resolver);
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(_defaultResolver, _resolversByScheme);
    }
}
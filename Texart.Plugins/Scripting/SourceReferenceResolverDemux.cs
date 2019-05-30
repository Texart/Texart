using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Texart.Api;

namespace Texart.Plugins.Scripting
{
    /// <summary>
    /// A source reference resolver that selects one of multiple resolvers based on a matching <see cref="TxReferenceScheme"/>.
    /// In the case where there is no matching scheme, it uses the default resolver as a fallback.
    /// </summary>
    public sealed class SourceReferenceResolverDemux : SourceReferenceResolver, IEquatable<SourceReferenceResolverDemux>
    {
        private readonly SourceReferenceResolver _defaultResolver;
        private readonly ImmutableDictionary<TxReferenceScheme, SourceReferenceResolver> _resolversByScheme;

        /// <summary>
        /// Creates an instance with a default fallback resolver, and a lookup table for different schemes.
        /// </summary>
        /// <param name="defaultResolver">The fallback resolvers.</param>
        /// <param name="resolversByScheme">The lookup table for different schemes.</param>
        public SourceReferenceResolverDemux(SourceReferenceResolver defaultResolver, ImmutableDictionary<TxReferenceScheme, SourceReferenceResolver> resolversByScheme)
        {
            _defaultResolver = defaultResolver ?? throw new ArgumentNullException(nameof(defaultResolver));
            _resolversByScheme = resolversByScheme ?? throw new ArgumentNullException(nameof(resolversByScheme));
        }

        /// <inheritdoc />
        public override string NormalizePath(string path, string baseFilePath)
        {
            var (scheme, resolver) = GetResolverByPath(path);
            var normalizedPath = resolver.NormalizePath(
                scheme != null ? scheme.NormalizePath(path) : path,
                baseFilePath);
            return scheme != null ? scheme.Prefix(normalizedPath) : normalizedPath;
        }

        /// <inheritdoc />
        public override string ResolveReference(string path, string baseFilePath)
        {
            var (scheme, resolver) = GetResolverByPath(path);
            var resolvedReferencePath = resolver.ResolveReference(
                scheme != null ? scheme.NormalizePath(path) : path,
                baseFilePath);
            return scheme != null ? scheme.Prefix(resolvedReferencePath) : resolvedReferencePath;
        }

        /// <inheritdoc />
        public override Stream OpenRead(string resolvedPath)
        {
            var (scheme, resolver) = GetResolverByPath(resolvedPath);
            return resolver.OpenRead(scheme != null ? scheme.NormalizePath(resolvedPath) : resolvedPath);
        }

        /// <summary>
        /// Gets the resolver by checking the provided path for a matching scheme.
        /// </summary>
        /// <param name="path">The path to check for scheme.</param>
        /// <returns>Matching resolver if found, else the default resolver.</returns>
        private (TxReferenceScheme, SourceReferenceResolver) GetResolverByPath(string path) =>
            _resolversByScheme
                .Where(kv => kv.Key.Matches(path))
                .Select(kv => (kv.Key, kv.Value))
                .DefaultIfEmpty((null, _defaultResolver))
                .First();

        /// <inheritdoc />
        public bool Equals(SourceReferenceResolverDemux other) =>
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
                case SourceReferenceResolverDemux resolver when resolver.GetType() == GetType():
                    return Equals(resolver);
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(_defaultResolver, _resolversByScheme);
    }
}

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Texart.Plugins.Scripting
{
    /// <summary>
    /// A reference resolver that selects one of multiple resolvers based on a matching <see cref="ReferenceScheme"/>.
    /// In the case where there is no matching scheme, it uses the default resolver as a fallback.
    /// </summary>
    public class SourceReferenceResolverDemux : SourceReferenceResolver, IEquatable<SourceReferenceResolverDemux>
    {
        private readonly SourceReferenceResolver _defaultResolver;
        private readonly IImmutableDictionary<ReferenceScheme, SourceReferenceResolver> _resolversByScheme;

        public SourceReferenceResolverDemux(SourceReferenceResolver defaultResolver, ImmutableDictionary<string, SourceReferenceResolver> resolversByScheme)
        {
            this._defaultResolver = defaultResolver ?? throw new ArgumentNullException(nameof(defaultResolver));
            this._resolversByScheme =
                ToSchemeKeyDictionary(resolversByScheme ?? throw new ArgumentNullException(nameof(resolversByScheme)));
        }

        public SourceReferenceResolverDemux(SourceReferenceResolver defaultResolver, ImmutableDictionary<ReferenceScheme, SourceReferenceResolver> resolversByScheme)
        {
            this._defaultResolver = defaultResolver ?? throw new ArgumentNullException(nameof(defaultResolver));
            this._resolversByScheme = resolversByScheme ?? throw new ArgumentNullException(nameof(resolversByScheme));
        }

        /// <inheritdoc />
        public override string NormalizePath(string path, string baseFilePath)
        {
            var (scheme, resolver) = GetResolverByPath(path);
            var normalizedPath = resolver.NormalizePath(
                scheme.HasValue ? scheme.Value.NormalizePath(path) : path,
                baseFilePath);
            return scheme.HasValue ? scheme.Value.Prefix(normalizedPath) : normalizedPath;
        }

        /// <inheritdoc />
        public override string ResolveReference(string path, string baseFilePath)
        {
            var (scheme, resolver) = GetResolverByPath(path);
            var resolvedReferencePath = resolver.ResolveReference(
                scheme.HasValue ? scheme.Value.NormalizePath(path) : path,
                baseFilePath);
            return scheme.HasValue ? scheme.Value.Prefix(resolvedReferencePath) : resolvedReferencePath;
        }

        /// <inheritdoc />
        public override Stream OpenRead(string resolvedPath)
        {
            var (scheme, resolver) = GetResolverByPath(resolvedPath);
            return resolver.OpenRead(scheme.HasValue ? scheme.Value.NormalizePath(resolvedPath) : resolvedPath);
        }

        private (ReferenceScheme?, SourceReferenceResolver) GetResolverByPath(string path) =>
            _resolversByScheme
                .Where(kv => kv.Key.Matches(path))
                .Select(kv => (new ReferenceScheme?(kv.Key), kv.Value))
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
                case SourceReferenceResolverDemux resolver when resolver.GetType() == this.GetType():
                    return this.Equals(resolver);
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(_defaultResolver, _resolversByScheme);

        private static ImmutableDictionary<ReferenceScheme, T> ToSchemeKeyDictionary<T>(ImmutableDictionary<string, T> dictionary)
        {
            return dictionary.ToImmutableDictionary(
                keyValuePair => new ReferenceScheme(keyValuePair.Key),
                keyValuePair => keyValuePair.Value);
        }

        private static ImmutableDictionary<TKey, TValue> EmptyIfNull<TKey, TValue>(ImmutableDictionary<TKey, TValue> dictionary) =>
            dictionary ?? ImmutableDictionary<TKey, TValue>.Empty;
    }
}

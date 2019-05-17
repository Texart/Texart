using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Texart.Scripting
{
    /// <summary>
    /// A reference resolver that selects one of multiple resolvers based on a matching <see cref="SourceReferenceScheme"/>.
    /// In the case where there is no matching scheme, it uses the default resolver as a fallback.
    /// </summary>
    public class SwitchedSourceReferenceResolver : SourceReferenceResolver, IEquatable<SwitchedSourceReferenceResolver>
    {
        private readonly SourceReferenceResolver _defaultResolver;
        private readonly IImmutableDictionary<SourceReferenceScheme, SourceReferenceResolver> _resolversByScheme;

        public SwitchedSourceReferenceResolver(SourceReferenceResolver defaultResolver, IImmutableDictionary<string, SourceReferenceResolver> resolversByScheme) :
            this(defaultResolver, ToSchemeKeyDictionary(resolversByScheme))
        {
        }

        public SwitchedSourceReferenceResolver(SourceReferenceResolver defaultResolver, IImmutableDictionary<SourceReferenceScheme, SourceReferenceResolver> resolversByScheme)
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

        private (SourceReferenceScheme?, SourceReferenceResolver) GetResolverByPath(string path) =>
            _resolversByScheme
                .Where(kv => kv.Key.Matches(path))
                .Select(kv => (new SourceReferenceScheme?(kv.Key), kv.Value))
                .DefaultIfEmpty((null, _defaultResolver))
                .First();

        /// <inheritdoc />
        public bool Equals(SwitchedSourceReferenceResolver other) =>
            _defaultResolver.Equals(other._defaultResolver) && _resolversByScheme.Equals(other._resolversByScheme);

        /// <inheritdoc />
        public override bool Equals(object other)
        {
            switch (other)
            {
                case SwitchedSourceReferenceResolver resolver when resolver.GetType() == this.GetType():
                    return this.Equals(resolver);
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(_defaultResolver, _resolversByScheme);

        private static IImmutableDictionary<SourceReferenceScheme, T> ToSchemeKeyDictionary<T>(IImmutableDictionary<string, T> dictionary) =>
            dictionary.ToImmutableDictionary(
                keyValuePair => new SourceReferenceScheme(keyValuePair.Key),
                keyValuePair => keyValuePair.Value);
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Texart.Api
{
    /// <summary>
    /// A domain-specific URI-like string that is used to identity a plugin assembly and objects inside it.
    /// The properties and formatting is a strict subset of a URI and thus all <see cref="TxPluginResourceLocator"/>s
    /// are valid (absolute) URIs. Relative resource URIs can be represented by <see cref="RelativeLocator"/>
    /// instead.
    ///
    /// <see href="https://tools.ietf.org/html/rfc3986#section-3"/> defines the syntax of URIs:
    /// <code>
    /// //    foo://example.com:8042/over/there?name=ferret#nose
    /// //    \_/   \______________/\_________/ \_________/ \__/
    /// //     |           |            |            |        |
    /// //  scheme     authority       path        query   fragment
    /// //     |   _____________________|__
    /// //    / \ /                        \
    /// //    urn:example:animal:ferret:nose
    /// </code>
    /// See <see href="https://en.wikipedia.org/wiki/Uniform_Resource_Identifier#/media/File:URI_syntax_diagram.png"/> for
    /// a simple diagram.
    ///
    /// From the above, only the following properties are allowed:
    ///   * scheme: <see cref="TxPluginResourceLocator.Scheme"/>.
    ///   * authority: This is required for a URI but in our case, this <b>MUST</b> be empty.
    ///   * path: The path <b>MUST</b> contain at least one segment containing <see cref="AssemblyResourceSeparator"/>,
    ///           the <i>last</i> of which is used to partition the path into <see cref="TxPluginResourceLocator.AssemblyPath"/>
    ///           and <see cref="TxPluginResourceLocator.ResourcePath"/>.
    ///           <see cref="TxPluginResourceLocator.ResourcePath"/> is allowed to be empty.
    /// These restrictions may be relaxed in the future, but will always be compliant with the latest URI RFC.
    /// Note that the current restrictions imply that http or https schemes cannot be used, since they require a hostname.
    /// </summary>
    /// <example>
    /// <code>
    ///     var locator = PluginResourceLocator.FromUri("file:///plugins/Texart.SomePlugin.dll:SomePath/SomeResource");
    ///     locator.Scheme;           // new ReferenceScheme("file")
    ///     locator.AssemblyPath      // "plugins/Texart.SomePlugin.dll
    ///     locator.AssemblySegments  // new [] { "plugins, "Texart.SomePlugin.dll" }
    ///     locator.ResourcePath      // "SomePath/SomeResource"
    ///     locator.ResourceSegments  // new [] { "SomePath", "SomeResource" }
    /// </code>
    /// </example>
    /// <seealso cref="RelativeLocator"/>
    public sealed class TxPluginResourceLocator : IEquatable<TxPluginResourceLocator>
    {
        /// <summary>
        /// The URI scheme.
        /// </summary>
        /// <seealso cref="Uri.Scheme"/>
        public TxReferenceScheme Scheme { get; }

        /// <summary>
        /// The segments in the URI path that locate the plugin <see cref="System.Reflection.Assembly"/> or script.
        /// </summary>
        public ImmutableArray<string> AssemblySegments { get; }
        /// <summary>
        /// <see cref="AssemblySegments"/> as a URI path string. The format is "path/to/plugin.dll".
        /// </summary>
        public string AssemblyPath => string.Join(UriPathSeparator, AssemblySegments);

        /// <summary>
        /// The resource path as a locator.
        /// </summary>
        public RelativeLocator RelativeResource { get; }
        /// <summary>
        /// The segments in the URI path that locate specific resource inside a plugin.
        /// For example: as a path to <see cref="ITxPlugin.LookupGenerator"/> or <see cref="ITxPlugin.LookupRenderer"/>.
        /// </summary>
        public ImmutableArray<string> ResourceSegments => RelativeResource.ResourceSegments;
        /// <summary>
        /// Backing field for <see cref="ResourcePath"/> which stored the cached value if it was previously computed.
        /// </summary>
        private string _resourcePathBackingField;
        /// <summary>
        /// <see cref="ResourceSegments"/> as a URI path string. The format is "path/to/resource".
        /// </summary>
        public string ResourcePath =>
            _resourcePathBackingField = _resourcePathBackingField ?? string.Join(UriPathSeparator, ResourceSegments);

        /// <summary>
        /// The URI representation of <c>this</c>.
        /// </summary>
        public Uri AsUri => new Uri(ToString(), UriKind.Absolute);

        /// <summary>
        /// <c>this</c> without <see cref="RelativeResource"/>.
        /// </summary>
        public TxPluginResourceLocator WithoutRelativeResource => WithRelativeResource(string.Empty);

        /// <summary>
        /// Character that is used to partition the URI path into <see cref="AssemblyPath"/> and <see cref="ResourcePath"/>.
        /// The choice of <c>:</c> is justified by its absence in file names (reasonably). Note that many user agents allow
        /// <c>:</c> in URI paths (e.g. web browsers). Paths containing <c>:</c> <i>may</i> need to be URI-encoded (<c>%3A</c>
        /// in this case).
        /// </summary>
        /// <seealso cref="Uri.EscapeUriString"/>
        public const char AssemblyResourceSeparator = ':';

        /// <summary>
        /// Constructs a <see cref="TxPluginResourceLocator"/> from an absolute URI.
        /// The provided URI must be valid: <see cref="CheckIsValidPluginResourceUri"/>.
        /// </summary>
        /// <param name="uri">The URI to build from.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="uri"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">If the URI is not valid.</exception>
        public static TxPluginResourceLocator Of(Uri uri) => new TxPluginResourceLocator(uri);

        /// <summary>
        /// Constructs a <see cref="TxPluginResourceLocator"/> from an absolute URI. The escaping semantics
        /// are defined by the <see cref="Uri"/> class.
        /// The provided URI must be valid: <see cref="CheckIsValidPluginResourceUri"/>.
        /// </summary>
        /// <param name="uri">The URI to build from.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="uri"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">If the URI is not valid.</exception>
        /// <seealso cref="Of(Uri)"/>
        /// <seealso cref="Uri(string, UriKind)"/>
        public static TxPluginResourceLocator Of(string uri) => Of(new Uri(uri, UriKind.Absolute));

        /// <summary>
        /// Determines if the provided URI is within the subset of allowed URIs.
        /// Not all URIs are valid <see cref="TxPluginResourceLocator"/>s.
        /// </summary>
        /// <param name="uri">The URI to check.</param>
        /// <returns>
        ///     <c>true</c> if valid <see cref="TxPluginResourceLocator"/> URI,
        ///     <c>false</c> otherwise.
        /// </returns>
        public static bool IsWellFormedResourceLocatorUri(Uri uri)
        {
            var (exception, _) = CheckIsValidPluginResourceUri(uri);
            return exception == null;
        }

        /// <summary>
        /// Constructs a <see cref="RelativeLocator"/> from the provided relative URI path. <paramref name="relativeResource"/>
        /// cannot contain <see cref="AssemblyResourceSeparator"/>. The escaping semantics are defined by the <see cref="Uri"/> class.
        /// </summary>
        /// <param name="relativeResource">The relative resource path. See <see cref="TxPluginResourceLocator.ResourcePath"/>.</param>
        /// <returns>A <see cref="RelativeLocator"/> from parsing <paramref name="relativeResource"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="relativeResource"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">If the relative resource path is not valid.</exception>
        public static RelativeLocator OfRelativeResource(string relativeResource)
        {
            var (checkFailedException, relative) = RelativeLocator.CheckIsValidRelativeResourcePath(relativeResource);
            if (checkFailedException != null)
            {
                throw checkFailedException;
            }
            return relative;
        }

        /// <summary>
        /// Determines if the given resource path is valid for <see cref="TxPluginResourceLocator"/>.
        /// </summary>
        /// <param name="relativeResource">The relative URI path to check.</param>
        /// <returns>
        ///     <c>true</c> if valid for <see cref="ResourcePath"/>,
        ///     <c>false</c> otherwise.
        /// </returns>
        public static bool IsWellFormedRelativeResourceString(string relativeResource)
        {
            var (checkFailedException, _) = RelativeLocator.CheckIsValidRelativeResourcePath(relativeResource);
            return checkFailedException == null;
        }

        /// <summary>
        /// Creates a new <see cref="TxPluginResourceLocator"/> with <see cref="Scheme"/> replaced with
        /// <paramref name="scheme"/>.
        /// </summary>
        /// <param name="scheme">The new <see cref="TxReferenceScheme"/>.</param>
        /// <returns>
        ///     A new <see cref="TxPluginResourceLocator"/> with <see cref="Scheme"/> replaced with
        ///     <paramref name="scheme"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="scheme"/> is <c>null</c></exception>
        /// <exception cref="FormatException">If the scheme is not valid.</exception>
        public TxPluginResourceLocator WithScheme(string scheme)
        {
            try
            {
                return WithScheme(new TxReferenceScheme(scheme));
            }
            catch (TxReferenceScheme.FormatException ex)
            {
                throw new FormatException($"URI scheme is not valid: {scheme}", ex);
            }
        }

        /// <summary>
        /// Creates a new <see cref="TxPluginResourceLocator"/> with <see cref="Scheme"/> replaced with
        /// <paramref name="scheme"/>.
        /// </summary>
        /// <param name="scheme">The new <see cref="TxReferenceScheme"/>.</param>
        /// <returns>
        ///     A new <see cref="TxPluginResourceLocator"/> with <see cref="Scheme"/> replaced with
        ///     <paramref name="scheme"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="scheme"/> is <c>null</c></exception>
        public TxPluginResourceLocator WithScheme(TxReferenceScheme scheme)
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }
            // also pass backing field since the resource path is not modified
            return new TxPluginResourceLocator(
                scheme, new ComputedSegments(AssemblySegments, RelativeResource), _resourcePathBackingField);
        }

        /// <summary>
        /// Creates a new <see cref="TxPluginResourceLocator"/> with <see cref="AssemblyPath"/> replaced with
        /// <paramref name="assemblyPath"/>. The escaping semantics are defined by the <see cref="Uri"/> class.
        /// </summary>
        /// <param name="assemblyPath">The new <see cref="AssemblyPath"/>.</param>
        /// <returns>
        ///     A new <see cref="TxPluginResourceLocator"/> with <see cref="AssemblyPath"/> replaced with
        ///     <paramref name="assemblyPath"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="assemblyPath"/> is <c>null</c></exception>
        /// <exception cref="FormatException">If the assembly path is not valid.</exception>
        public TxPluginResourceLocator WithAssemblyPath(string assemblyPath)
        {
            var (assemblySegmentsException, assemblySegments) = CheckIsValidAssemblyPath(assemblyPath);
            if (assemblySegmentsException != null)
            {
                throw assemblySegmentsException;
            }
            // also pass backing field since the resource path is not modified
            return new TxPluginResourceLocator(
                Scheme, new ComputedSegments(assemblySegments, RelativeResource), _resourcePathBackingField);
        }

        /// <summary>
        /// Creates a new <see cref="TxPluginResourceLocator"/> with <see cref="RelativeResource"/> replaced with
        /// <paramref name="relativeResourceLocator"/>. The escaping semantics are defined by the <see cref="Uri"/> class.
        /// </summary>
        /// <param name="relativeResourceLocator">The new <see cref="RelativeLocator"/>.</param>
        /// <returns>
        ///     A new <see cref="TxPluginResourceLocator"/> with <see cref="ResourcePath"/> replaced with
        ///     <paramref name="relativeResourceLocator"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="relativeResourceLocator"/> is <c>null</c></exception>
        /// <exception cref="FormatException">If the resource path is not valid.</exception>
        public TxPluginResourceLocator WithRelativeResource(string relativeResourceLocator) =>
            WithRelativeResource(OfRelativeResource(relativeResourceLocator));

        /// <summary>
        /// Creates a new <see cref="TxPluginResourceLocator"/> with <see cref="RelativeResource"/> replaced with
        /// <paramref name="relativeLocator"/>.
        /// </summary>
        /// <param name="relativeLocator"></param>
        /// <returns>
        ///     A new <see cref="TxPluginResourceLocator"/> with <see cref="ResourcePath"/> replaced with
        ///     <paramref name="relativeLocator"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="relativeLocator"/> is <c>null</c></exception>
        public TxPluginResourceLocator WithRelativeResource(RelativeLocator relativeLocator)
        {
            if (relativeLocator == null)
            {
                throw new ArgumentNullException(nameof(relativeLocator));
            }
            // the resource path needs to be recalculated, so don't pass backing field
            return new TxPluginResourceLocator(
                Scheme, new ComputedSegments(AssemblySegments, relativeLocator), null);
        }

        /// <summary>
        /// A <see cref="RelativeLocator"/> represents the <see cref="ResourcePath"/> part of <see cref="TxPluginResourceLocator"/>.
        /// It represents a resource relative to <see cref="AssemblyPath"/>.
        /// </summary>
        public sealed class RelativeLocator : IEquatable<RelativeLocator>
        {
            /// <summary>
            /// See <see cref="TxPluginResourceLocator.ResourceSegments"/>.
            /// </summary>
            public ImmutableArray<string> ResourceSegments { get; }

            /// <summary>
            /// <see cref="ResourceSegments"/> as a path string.
            /// </summary>
            public string ResourcePath => string.Join(UriPathSeparator, ResourceSegments);

            /// <summary>
            /// Constructs a <see cref="RelativeLocator"/> from the provided segments. The arguments are not checked to see if
            /// they are valid.
            /// </summary>
            /// <param name="resourceSegments">See <see cref="ResourceSegments"/>.</param>
            internal RelativeLocator(ImmutableArray<string> resourceSegments)
            {
                Debug.Assert(resourceSegments != null);
                ResourceSegments = resourceSegments;
            }

            /// <summary>
            /// Makes sure that the given path is valid for <see cref="TxPluginResourceLocator.ResourcePath"/>.
            /// </summary>
            /// <param name="relativePath">The relative URI path to check.</param>
            /// <returns>An exception if the URI is invalid, or <see cref="RelativeLocator"/> if valid.</returns>
            /// <seealso cref="TxPluginResourceLocator.CheckIsValidPluginResourceUri"/>
            internal static (Exception, RelativeLocator) CheckIsValidRelativeResourcePath(string relativePath)
            {
                if (relativePath == null)
                {
                    return (new ArgumentNullException(nameof(relativePath)), default);
                }

                if (relativePath.Contains(AssemblyResourceSeparator))
                {
                    return (
                        new FormatException(
                            $"URI relative resource must not contain \"{AssemblyResourceSeparator}\": {relativePath}"),
                        default);
                }

                // We temporarily prefix with a dummy character so that leading slashes don't get discarded by Uri
                const char dummyPrefixChar = AssemblyResourceSeparator;
                var prefixedRelativePath = $"{dummyPrefixChar}{relativePath}";

                if (!Uri.TryCreate(prefixedRelativePath, UriKind.Relative, out var relativeUri) ||
                    !Uri.TryCreate(DummyBaseUri, relativeUri, out var absoluteUri))
                {
                    return (new FormatException($"URI path must be valid relative URI: {relativePath}"), default);
                }
                Debug.Assert(absoluteUri.IsAbsoluteUri);

                // Query check
                if (!string.IsNullOrEmpty(absoluteUri.Query))
                {
                    return (new FormatException($"URI query is not allowed: {absoluteUri.Query}"), default);
                }

                // Fragment check
                if (!string.IsNullOrEmpty(absoluteUri.Fragment))
                {
                    return (new FormatException($"URI fragment is not allowed: {absoluteUri.Fragment}"), default);
                }

                // Path check
                var expectedPathPrefix = $"{UriPathSeparator}{dummyPrefixChar}";
                var absolutePath = absoluteUri.AbsolutePath;
                Debug.Assert(absolutePath.StartsWith(expectedPathPrefix));
                var relative = ComputeRelative(absolutePath.Substring(expectedPathPrefix.Length));

                // All good!
                return (null, relative);
            }

            /// <summary>
            /// Creates a <see cref="RelativeLocator"/> from the URI path, <paramref name="path"/>.
            /// </summary>
            /// <param name="path">The path to sanitize.</param>
            /// <returns>Computed relative resource path.</returns>
            private static RelativeLocator ComputeRelative(string path)
            {
                Debug.Assert(path != null);
                Debug.Assert(!path.Contains(AssemblyResourceSeparator));
                return new RelativeLocator(
                    path.Split(UriPathSeparator).ToImmutableArray());
            }

            /// <inheritdoc cref="object.ToString"/>
            public override string ToString() => ResourcePath;

            /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
            public bool Equals(RelativeLocator other)
            {
                if (ReferenceEquals(null, other)) return false;
                return ReferenceEquals(this, other) || ResourceSegments.SequenceEqual(other.ResourceSegments);
            }

            /// <inheritdoc cref="object.Equals(object)"/>
            public override bool Equals(object obj) =>
                ReferenceEquals(this, obj) || obj is RelativeLocator other && Equals(other);

            /// <inheritdoc cref="object.GetHashCode"/>
            public override int GetHashCode()
            {
                var hashCode = new HashCode();
                foreach (var segment in ResourceSegments)
                {
                    hashCode.Add(segment);
                }
                return hashCode.ToHashCode();
            }

            /// <summary>
            /// Compares two <see cref="RelativeLocator"/> for equality.
            /// </summary>
            /// <param name="lhs">The left hand side of the equality.</param>
            /// <param name="rhs">The right hand side of the equality.</param>
            /// <returns>Whether the two instances refer to the same resource paths or not.</returns>
            public static bool operator ==(RelativeLocator lhs, RelativeLocator rhs) => Equals(lhs, rhs);

            /// <summary>
            /// Compares two <see cref="RelativeLocator"/> for inequality.
            /// </summary>
            /// <param name="lhs">The left hand side of the inequality.</param>
            /// <param name="rhs">The right hand side of the inequality.</param>
            /// <returns>Whether the two instances refer to different resource paths or not.</returns>
            public static bool operator !=(RelativeLocator lhs, RelativeLocator rhs) => !(lhs == rhs);
        }

        /// <summary>
        /// A <see cref="ComputedSegments"/> represents the <see cref="Uri.AbsolutePath"/> part of
        /// <see cref="TxPluginResourceLocator"/>. Specifically, <see cref="AssemblySegments"/> and
        /// <see cref="ResourceSegments"/>.
        /// </summary>
        private readonly struct ComputedSegments
        {
            /// <summary>
            /// See <see cref="TxPluginResourceLocator.AssemblySegments"/>.
            /// </summary>
            public ImmutableArray<string> AssemblySegments { get; }
            /// <summary>
            /// See <see cref="TxPluginResourceLocator.RelativeResource"/>.
            /// </summary>
            public RelativeLocator RelativeResource { get; }
            /// <summary>
            /// Constructs a <see cref="ComputedSegments"/> with pre-computed values.
            /// </summary>
            /// <param name="assemblySegments">The pre-computed assembly path segments.</param>
            /// <param name="relativeResource">The pre-computed relative resource path.</param>
            public ComputedSegments(ImmutableArray<string> assemblySegments, RelativeLocator relativeResource)
            {
                Debug.Assert(assemblySegments != null);
                Debug.Assert(relativeResource != null);
                AssemblySegments = assemblySegments;
                RelativeResource = relativeResource;
            }
        }
        /// <summary>
        /// Makes sure that the given URI is within the subset of allowed URIs.
        /// Not all URIs are valid <see cref="TxPluginResourceLocator"/>s.
        /// </summary>
        /// <param name="uri">The URI to check.</param>
        /// <returns>An exception if the URI is invalid, or <see cref="RelativeLocator"/> if valid.</returns>
        private static (Exception, ComputedSegments) CheckIsValidPluginResourceUri(Uri uri)
        {
            //
            // Reference: https://tools.ietf.org/html/rfc3986#section-3
            //
            //     foo://example.com:8042/over/there?name=ferret#nose
            //     \_/   \______________/\_________/ \_________/ \__/
            //      |           |            |            |        |
            //   scheme     authority       path        query   fragment
            //      |   _____________________|__
            //     / \ /                        \
            //     urn:example:animal:ferret:nose
            //
            if (uri == null) { return (new ArgumentNullException(nameof(uri)), default); }
            if (!uri.IsAbsoluteUri)
            {
                return (new FormatException($"URI must be absolute: {uri}"), default);
            }

            // Scheme check
            // Reference: https://tools.ietf.org/html/rfc3986#section-3.1
            // scheme      = ALPHA *( ALPHA / DIGIT / "+" / "-" / "." )
            if (!TxReferenceScheme.IsValidScheme(uri.Scheme))
            {
                return (new FormatException($"URI reference scheme is invalid: {uri.Scheme}"), default);
            }

            // Authority check
            // Reference: https://tools.ietf.org/html/rfc3986#section-3.2
            //   authority   = [ userinfo "@" ] host [ ":" port ]
            if (!string.IsNullOrEmpty(uri.Authority))
            {
                return (new FormatException($"URI authority is not allowed: ${uri.Authority}"), default);
            }

            // Query check
            if (!string.IsNullOrEmpty(uri.Query))
            {
                return (new FormatException($"URI query is not allowed: {uri.Query}"), default);
            }

            // Fragment check
            if (!string.IsNullOrEmpty(uri.Fragment))
            {
                return (new FormatException($"URI fragment is not allowed: {uri.Fragment}"), default);
            }

            // Path check
            var (computeSegmentException, segments) = ComputeSegments(uri.AbsolutePath);
            if (computeSegmentException != null)
            {
                return (computeSegmentException, default);
            }

            // All good!
            return (null, segments);
        }

        /// <summary>
        /// Creates a <see cref="ComputedSegments"/> from segment parts, or an <see cref="Exception"/> on failure.
        /// </summary>
        /// <param name="path">The path to partition.</param>
        /// <returns>Computed partition.</returns>
        private static (Exception, ComputedSegments) ComputeSegments(string path)
        {
            Debug.Assert(path != null);
            ReadOnlySpan<string> segments = path.Split(UriPathSeparator);
            if (!segments.IsEmpty && segments[0] == string.Empty)
            {
                // Remove trailing /
                segments = segments.Slice(1);
            }

            var assemblySegments = new List<string>();
            var resourceSegments = new List<string>();
            bool separatorFound = false;
            // reverse the segments: the partition of assembly path and resource path is based on the last occurence of the separator
            for (var i = segments.Length - 1; i >= 0; --i)
            {
                var segment = segments[i];
                if (separatorFound)
                {
                    assemblySegments.Add(segment);
                }
                else
                {
                    var separatorIndex = segment.LastIndexOf(AssemblyResourceSeparator);
                    if (separatorIndex != -1)
                    {
                        separatorFound = true;
                        assemblySegments.Add(segment.Substring(0, separatorIndex));
                        resourceSegments.Add(segment.Substring(separatorIndex + 1));
                        //if (separatorIndex != segment.Length - 1)
                        //{
                        //    // only add non-empty strings
                        //    var resourceSegment = segment.Substring(separatorIndex + 1);
                        //    Debug.Assert(resourceSegment != string.Empty);
                        //    resourceSegments.Add(resourceSegment);
                        //}
                    }
                    else
                    {
                        resourceSegments.Add(segment);
                    }
                }
            }

            if (!separatorFound)
            {
                var exception = new FormatException(
                    $"URI path did not contain assembly-resource separator ('{AssemblyResourceSeparator}'): {path}");
                return (exception, default);
            }

            assemblySegments.Reverse();
            resourceSegments.Reverse();

            // No need to recompute resource segments again, but the results must be consistent.
            Debug.Assert(IsWellFormedRelativeResourceString(string.Join(UriPathSeparator, resourceSegments)));
            // Make sure that the choice not to escape is valid.
            Debug.Assert(!resourceSegments.Any(segment => segment.Contains(AssemblyResourceSeparator)));

            return (null, new ComputedSegments(
                assemblySegments.ToImmutableArray(), new RelativeLocator(resourceSegments.ToImmutableArray())));
        }

        /// <summary>
        /// A <see cref="FormatException"/> is thrown when an attempt is made to create <see cref="TxPluginResourceLocator"/>
        /// or <see cref="RelativeLocator"/> with an invalid string.
        /// </summary>
        public sealed class FormatException : System.FormatException
        {
            /// <summary>
            /// Creates an exception with <see cref="Exception.Message"/> set to <paramref name="message"/>.
            /// </summary>
            /// <param name="message">The exception message.</param>
            internal FormatException(string message) : base(message) { }
            /// <summary>
            /// Creates an exception with <see cref="Exception.Message"/> set to <paramref name="message"/>
            /// and <see cref="Exception.InnerException"/> set to <paramref name="innerException"/>.
            /// </summary>
            /// <param name="message">The exception message.</param>
            /// <param name="innerException">The inner exception that caused this.</param>
            internal FormatException(string message, Exception innerException) : base(message, innerException) { }
        }

        /// <summary>
        /// Constructs a <see cref="TxPluginResourceLocator"/> from a URI.
        /// The provided URI must be valid: <see cref="CheckIsValidPluginResourceUri"/>.
        /// </summary>
        /// <param name="uri">The URI to parse.</param>
        private TxPluginResourceLocator(Uri uri)
        {
            var (checkFailedException, computedSegments) = CheckIsValidPluginResourceUri(uri);
            if (checkFailedException != null)
            {
                throw checkFailedException;
            }
            Debug.Assert(uri.IsAbsoluteUri);
            Scheme = new TxReferenceScheme(uri.Scheme);
            AssemblySegments = computedSegments.AssemblySegments;
            RelativeResource = computedSegments.RelativeResource;
        }

        /// <summary>
        /// Constructs a <see cref="TxPluginResourceLocator"/> with the specified components.
        /// </summary>
        /// <param name="scheme">The new URI scheme</param>
        /// <param name="computedSegments">Precomputed <see cref="AssemblyPath"/> and <see cref="RelativeResource"/></param>
        /// <param name="resourcePathBackingField">
        ///     Optimization that allows setting <see cref="_resourcePathBackingField"/> if it remains unchanged from a
        ///     previous object. Be careful not to set this if the resource path has changed!
        /// </param>
        private TxPluginResourceLocator(TxReferenceScheme scheme, ComputedSegments computedSegments,
            string resourcePathBackingField)
        {
            Debug.Assert(scheme != null);
            Scheme = scheme;
            AssemblySegments = computedSegments.AssemblySegments;
            RelativeResource = computedSegments.RelativeResource;
            _resourcePathBackingField = resourcePathBackingField;
        }

        /// <summary>
        /// Path separator in a URI. Specified in <see href="https://tools.ietf.org/html/rfc3986"/>.
        /// </summary>
        private static char UriPathSeparator => '/';

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() =>
            $"{Scheme}{Uri.SchemeDelimiter}{UriPathSeparator}{AssemblyPath}{AssemblyResourceSeparator}{ResourcePath}";

        /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
        public bool Equals(TxPluginResourceLocator other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Scheme.Equals(other.Scheme) &&
                   AssemblySegments.SequenceEqual(other.AssemblySegments) &&
                   RelativeResource == other.RelativeResource;
        }

        /// <inheritdoc cref="object.Equals(object)"/>
        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is TxPluginResourceLocator other && Equals(other);

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Scheme);
            foreach (var segment in AssemblySegments)
            {
                hashCode.Add(segment);
            }
            foreach (var segment in ResourceSegments)
            {
                hashCode.Add(segment);
            }
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Compares two <see cref="TxPluginResourceLocator"/> for equality.
        /// </summary>
        /// <param name="lhs">The left hand side of the equality.</param>
        /// <param name="rhs">The right hand side of the equality.</param>
        /// <returns>Whether the two instances refer to the same URIs or not.</returns>
        public static bool operator ==(TxPluginResourceLocator lhs, TxPluginResourceLocator rhs) => Equals(lhs, rhs);

        /// <summary>
        /// Compares two <see cref="TxPluginResourceLocator"/> for inequality.
        /// </summary>
        /// <param name="lhs">The left hand side of the inequality.</param>
        /// <param name="rhs">The right hand side of the inequality.</param>
        /// <returns>Whether the two instances refer to different URIs or not.</returns>
        public static bool operator !=(TxPluginResourceLocator lhs, TxPluginResourceLocator rhs) => !(lhs == rhs);

        /// <summary>
        /// Makes sure that the given path is valid for <see cref="AssemblyPath"/>.
        /// </summary>
        /// <param name="assemblyPath">The assembly URI path to check.</param>
        /// <returns>An exception if the URI is invalid, or <see cref="ImmutableArray{T}"/> if valid.</returns>
        /// <seealso cref="TxPluginResourceLocator.CheckIsValidPluginResourceUri"/>
        private static (Exception, ImmutableArray<string>) CheckIsValidAssemblyPath(string assemblyPath)
        {
            //
            // Reference: https://tools.ietf.org/html/rfc3986#section-3
            //
            //     foo://example.com:8042/over/there?name=ferret#nose
            //     \_/   \______________/\_________/ \_________/ \__/
            //      |           |            |            |        |
            //   scheme     authority       path        query   fragment
            //      |   _____________________|__
            //     / \ /                        \
            //     urn:example:animal:ferret:nose
            //
            if (assemblyPath == null)
            {
                return (new ArgumentNullException(nameof(assemblyPath)), default);
            }

            // We temporarily prefix with a dummy character so that leading slashes don't get discarded by Uri
            const char dummyPrefixChar = AssemblyResourceSeparator;
            var prefixedAssemblyPath = $"{dummyPrefixChar}{assemblyPath}";

            if (!Uri.TryCreate(prefixedAssemblyPath, UriKind.Relative, out var relativeUri) ||
                !Uri.TryCreate(DummyBaseUri, relativeUri, out var absoluteUri))
            {
                return (new FormatException($"URI path must be valid relative URI: {assemblyPath}"), default);
            }
            Debug.Assert(absoluteUri.IsAbsoluteUri);

            // Query check
            if (!string.IsNullOrEmpty(absoluteUri.Query))
            {
                return (new FormatException($"URI query is not allowed: {absoluteUri.Query}"), default);
            }

            // Fragment check
            if (!string.IsNullOrEmpty(absoluteUri.Fragment))
            {
                return (new FormatException($"URI fragment is not allowed: {absoluteUri.Fragment}"), default);
            }

            // Path check
            var expectedPathPrefix = $"{UriPathSeparator}{dummyPrefixChar}";
            var absolutePath = absoluteUri.AbsolutePath;
            Debug.Assert(absolutePath.StartsWith(expectedPathPrefix));
            var assemblySegments = ComputeAssemblySegments(absolutePath.Substring(expectedPathPrefix.Length));

            // All good!
            return (null, assemblySegments);
        }

        /// <summary>
        /// Creates a <see cref="ImmutableArray{T}"/> for <see cref="AssemblySegments"/> from the URI path,
        /// <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to sanitize.</param>
        /// <returns>Computed assembly path.</returns>
        private static ImmutableArray<string> ComputeAssemblySegments(string path)
        {
            Debug.Assert(path != null);
            return path.Split(UriPathSeparator).ToImmutableArray();
        }

        /// <summary>
        /// An absolute URI that is used to create dummy URIs from relative URIs.
        /// </summary>
        private static readonly Uri DummyBaseUri = new Uri("dummy:///", UriKind.Absolute);
    }
}
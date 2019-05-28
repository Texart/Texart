using System;
using System.Collections.Generic;
using System.Diagnostics;
using Texart.Api;

namespace Texart.Plugins
{
    /// <summary>
    /// A domain-specific URI-like string that is used to identity a plugin assembly and objects inside it.
    /// The properties and formatting is a strict subset of a URI and thus all <see cref="PluginResourceLocator"/>s
    /// are valid (absolute) URIs.
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
    ///   * scheme: <see cref="PluginResourceLocator.Scheme"/>.
    ///   * authority: This is required for a URI but in our case, this <b>MUST</b> be empty.
    ///   * path: The path <b>MUST</b> contain at least one segment containing <see cref="AssemblyResourceSeparator"/>,
    ///           the <i>last</i> of which is used to partition the path into <see cref="PluginResourceLocator.AssemblyPath"/>
    ///           and <see cref="PluginResourceLocator.ResourcePath"/>.
    ///           <see cref="PluginResourceLocator.ResourcePath"/> is allowed to be empty.
    /// These restrictions may be relaxed in the future, but will always be compliant with the latest URI RFC.
    ///
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
    /// </summary>
    public sealed class PluginResourceLocator : IEquatable<PluginResourceLocator>
    {
        /// <summary>
        /// The URI scheme. See <see cref="Uri.Scheme"/>.
        /// </summary>
        public ReferenceScheme Scheme => new ReferenceScheme(AsUri.Scheme);

        /// <summary>
        /// The segments in the URI path that locate the plugin <see cref="System.Reflection.Assembly"/> or <see cref="Scripting.SourceFile"/>.
        /// </summary>
        public string[] AssemblySegments { get; }
        /// <summary>
        /// <see cref="AssemblySegments"/> as a path string.
        /// </summary>
        public string AssemblyPath => string.Join(UriPathSeparator, AssemblySegments);

        /// <summary>
        /// The segments in the URI path that locate specific resource inside a plugin.
        /// For example: as a path to <see cref="ITxPlugin.LookupGenerator"/> or <see cref="ITxPlugin.LookupRenderer"/>.
        /// </summary>
        public string[] ResourceSegments { get; }
        /// <summary>
        /// <see cref="ResourceSegments"/> as a path string.
        /// </summary>
        public string ResourcePath => string.Join(UriPathSeparator, ResourceSegments);

        /// <summary>
        /// The backing URI object.
        /// </summary>
        public Uri AsUri { get; }

        /// <summary>
        /// Character that is used to partition the URI path into <see cref="AssemblyPath"/> and <see cref="ResourcePath"/>.
        /// The choice of <c>:</c> is justified by its absence in file names (reasonably). Note that many user agents allow
        /// <c>:</c> in URI paths (e.g. web browsers). Paths containing <c>:</c> <i>may</i> need to be encoded (<c>%3A</c>
        /// in this case).
        /// </summary>
        public const char AssemblyResourceSeparator = ':';

        /// <summary>
        /// Constructs a <see cref="PluginResourceLocator"/> with a backing URI.
        /// The provided URI must be valid: <see cref="CheckIsValidPluginResourceUri"/>.
        /// </summary>
        /// <param name="uri">The URI to build from.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="uri"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If the URI is not valid.</exception>
        public static PluginResourceLocator FromUri(Uri uri) => new PluginResourceLocator(uri);

        /// <summary>
        /// Constructs a <see cref="PluginResourceLocator"/> with a backing URI.
        /// The provided URI must be valid: <see cref="CheckIsValidPluginResourceUri"/>.
        /// </summary>
        /// <param name="uri">The URI to build from.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="uri"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If the URI is not valid.</exception>
        public static PluginResourceLocator FromUri(string uri) => FromUri(new Uri(uri));

        /// <summary>
        /// Determines if the provided URI is within the subset of allowed URIs.
        /// Not all URIs are valid <see cref="PluginResourceLocator"/>s.
        /// </summary>
        /// <param name="uri">The URI to check.</param>
        /// <returns>
        ///     <c>true</c> if valid <see cref="PluginResourceLocator"/> URI,
        ///     <c>false</c> otherwise.
        /// </returns>
        public static bool IsValidPluginResourceLocator(Uri uri)
        {
            var (exception, _) = CheckIsValidPluginResourceUri(uri);
            return exception == null;
        }

        /// <summary>
        /// Constructs a <see cref="PluginResourceLocator"/> with a backing URI.
        /// The provided URI must be valid: <see cref="CheckIsValidPluginResourceUri"/>.
        /// </summary>
        /// <param name="uri">The backing URI.</param>
        private PluginResourceLocator(Uri uri)
        {
            var (checkFailedException, precomputed) = CheckIsValidPluginResourceUri(uri);
            if (checkFailedException != null)
            {
                throw checkFailedException;
            }
            AsUri = uri;
            AssemblySegments = precomputed.AssemblySegments;
            ResourceSegments = precomputed.ResourceSegments;
        }

        /// <summary>
        /// Precomputed values for the constructor to use.
        /// </summary>
        private readonly struct ComputedSegments
        {
            /// <summary>
            /// See <see cref="PluginResourceLocator.AssemblySegments"/>.
            /// </summary>
            public string[] AssemblySegments { get; }
            /// <summary>
            /// See <see cref="PluginResourceLocator.ResourceSegments"/>.
            /// </summary>
            public string[] ResourceSegments { get; }
            /// <summary>
            /// Constructs from precomputed values.
            /// </summary>
            /// <param name="assemblySegments"><see cref="AssemblySegments"/></param>
            /// <param name="resourceSegments"><see cref="ResourceSegments"/></param>
            public ComputedSegments(string[] assemblySegments, string[] resourceSegments)
            {
                Debug.Assert(assemblySegments != null);
                Debug.Assert(resourceSegments != null);
                AssemblySegments = assemblySegments;
                ResourceSegments = resourceSegments;
            }
        }
        /// <summary>
        /// Makes sure that the given URI is within the subset of allowed URIs.
        /// Not all URIs are valid <see cref="PluginResourceLocator"/>s.
        /// </summary>
        /// <param name="uri">The URI to check.</param>
        /// <returns>An exception if the URI is invalid, or <see cref="ComputedSegments"/> if valid.</returns>
        private static (ArgumentException, ComputedSegments) CheckIsValidPluginResourceUri(Uri uri)
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
                return (new ArgumentException($"URI must be absolute: {uri}"), default);
            }

            // Scheme check
            // Reference: https://tools.ietf.org/html/rfc3986#section-3.1
            // scheme      = ALPHA *( ALPHA / DIGIT / "+" / "-" / "." )
            if (!ReferenceScheme.IsValidScheme(uri.Scheme))
            {
                return (new ArgumentException($"URI reference scheme is invalid: {uri.Scheme}"), default);
            }

            // Authority check
            // Reference: https://tools.ietf.org/html/rfc3986#section-3.2
            //   authority   = [ userinfo "@" ] host [ ":" port ]
            if (!string.IsNullOrEmpty(uri.Authority))
            {
                return (new ArgumentException($"URI authority is not allowed: ${uri.UserInfo}"), default);
            }

            // Query check
            if (!string.IsNullOrEmpty(uri.Query))
            {
                return (new ArgumentException($"URI query is not allowed: {uri.Query}"), default);
            }

            // Fragment check
            if (!string.IsNullOrEmpty(uri.Fragment))
            {
                return (new ArgumentException($"URI fragment is not allowed: {uri.Fragment}"), default);
            }

            // Path check
            var (computeSegmentException, segments) = ComputeSegments(uri.PathAndQuery);
            if (computeSegmentException != null)
            {
                return (computeSegmentException, default);
            }

            // All good!
            return (null, segments);
        }
        /// <summary>
        /// Creates a <see cref="ComputedSegments"/> from segment parts.
        /// </summary>
        /// <param name="path">The path to partition.</param>
        /// <returns>Computed partition.</returns>
        private static (ArgumentException, ComputedSegments) ComputeSegments(string path)
        {
            ReadOnlySpan<string> segments = path.Split(UriPathSeparator);
            if (!segments.IsEmpty && segments[0] == string.Empty)
            {
                // Remove trailing /
                segments = segments.Slice(1);
            }

            var assemblySegments = new List<string>();
            var resourceSegments = new List<string>();
            bool separatorFound = false;
            // reverse the segments - the partition is based on the last occurence of the separator
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
                        assemblySegments.Add(segment.Substring(0, separatorIndex));
                        resourceSegments.Add(segment.Substring(separatorIndex + 1));
                        separatorFound = true;
                    }
                    else
                    {
                        resourceSegments.Add(segment);
                    }
                }
            }

            if (!separatorFound)
            {
                var exception = new ArgumentException(
                    $"URI path did not contain assembly-resource separator ('{AssemblyResourceSeparator})': {path}");
                return (exception, default);
            }

            assemblySegments.Reverse();
            resourceSegments.Reverse();

            return (null, new ComputedSegments(assemblySegments.ToArray(), resourceSegments.ToArray()));
        }

        /// <summary>
        /// Path separator in a URI. Specified in <see href="https://tools.ietf.org/html/rfc3986"/>.
        /// </summary>
        private static char UriPathSeparator => '/';

        /// <inheritdoc />
        public override string ToString() => AsUri.ToString();

        /// <inheritdoc />
        public bool Equals(PluginResourceLocator other)
        {
            if (other is null) return false;
            return ReferenceEquals(this, other) || AsUri.Equals(other.AsUri);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is PluginResourceLocator other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => AsUri.GetHashCode();

        /// <summary>
        /// Compares two <see cref="PluginResourceLocator"/> for equality.
        /// </summary>
        /// <param name="lhs">The left hand side of the equality.</param>
        /// <param name="rhs">The right hand side of the equality.</param>
        /// <returns>Whether the two instances refer to the same URIs or not.</returns>
        public static bool operator ==(PluginResourceLocator lhs, PluginResourceLocator rhs) => Equals(lhs, rhs);

        /// <summary>
        /// Compares two <see cref="PluginResourceLocator"/> for inequality.
        /// </summary>
        /// <param name="lhs">The left hand side of the inequality.</param>
        /// <param name="rhs">The right hand side of the inequality.</param>
        /// <returns>Whether the two instances refer to different URIs or not.</returns>
        public static bool operator !=(PluginResourceLocator lhs, PluginResourceLocator rhs) => !(lhs == rhs);
    }
}
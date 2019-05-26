using System;

namespace Texart.Plugins
{
    /// <summary>
    /// A URI-like string that is used to identity objects inside a plugin assembly.
    /// The properties and formatting is a strict subset of a URI and thus: all <code>PluginResourceLocator</code>s
    /// are valid (absolute) URIs.
    ///
    /// <see href="https://tools.ietf.org/html/rfc3986#section-3"/> defines the syntax of URIs:
    /// <code>
    ///     foo://example.com:8042/over/there?name=ferret#nose
    ///     \_/   \______________/\_________/ \_________/ \__/
    ///      |           |            |            |        |
    ///   scheme     authority       path        query   fragment
    ///      |   _____________________|__
    ///     / \ /                        \
    ///     urn:example:animal:ferret:nose
    /// </code>
    /// See <see href="https://en.wikipedia.org/wiki/Uniform_Resource_Identifier#/media/File:URI_syntax_diagram.png"/> for
    /// a simple diagram.
    ///
    /// From the above, only the following properties are allowed:
    ///   * scheme: <see cref="PluginResourceLocator.Scheme"/>.
    ///   * authority: <see cref="PluginResourceLocator.Host"/>. The port is not allowed, however.
    ///   * path: <see cref="PluginResourceLocator.Segments"/> and <see cref="PluginResourceLocator.Path"/>.
    /// These restrictions may be relaxed in the future, but will always be compliant with the latest URI RFC.
    /// </summary>
    public sealed class PluginResourceLocator : IEquatable<PluginResourceLocator>
    {
        /// <summary>
        /// The URI scheme. See <see cref="Uri.Scheme"/>.
        /// </summary>
        public ReferenceScheme Scheme => new ReferenceScheme(AsUri.Scheme);
        /// <summary>
        /// The URI authority. See <see cref="Uri.Host"/>.
        /// </summary>
        public string Host => AsUri.Host;
        /// <summary>
        /// The URI path. See <see cref="Uri.Segments"/>.
        /// </summary>
        public string Path => AsUri.PathAndQuery;
        /// <summary>
        /// The URI path segments. See <see cref="Uri.Segments"/>
        /// </summary>
        public string[] Segments => AsUri.Segments;
        /// <summary>
        /// The backing URI object.
        /// </summary>
        public Uri AsUri { get; }

        /// <summary>
        /// Constructs a <see cref="PluginResourceLocator"/> with a backing URI.
        /// The provided URI must be valid: <see cref="CheckIsValidPluginResourceUri"/>.
        /// </summary>
        /// <param name="uri">The backing URI.</param>
        private PluginResourceLocator(Uri uri)
        {
            var checkFailedException = CheckIsValidPluginResourceUri(uri);
            if (checkFailedException != null)
            {
                throw checkFailedException;
            }
            AsUri = uri;
        }

        /// <summary>
        /// Constructs a <see cref="PluginResourceLocator"/> with a backing URI.
        /// The provided URI must be valid: <see cref="CheckIsValidPluginResourceUri"/>.
        /// </summary>
        /// <param name="uri">The URI to build from.</param>
        public static PluginResourceLocator FromUri(Uri uri) => new PluginResourceLocator(uri);

        /// <summary>
        /// Determines if the provided URI is within the subset of allowed URIs.
        /// Not all URIs are valid <see cref="PluginResourceLocator"/>s.
        /// </summary>
        /// <param name="uri">The URI to check.</param>
        /// <returns>
        ///     <code>true</code> if valid <see cref="PluginResourceLocator"/> URI,
        ///     <code>false</code> otherwise.
        /// </returns>
        public static bool IsValidResourceLocator(Uri uri) => CheckIsValidPluginResourceUri(uri) == null;

        /// <summary>
        /// Makes sure that the given URI is within the subset of allowed URIs.
        /// Not all URIs are valid <see cref="PluginResourceLocator"/>s.
        /// </summary>
        /// <param name="uri">The URI to check.</param>
        /// <returns>An exception if the URI is invalid, or <code>null</code> if valid.</returns>
        private static ArgumentException CheckIsValidPluginResourceUri(Uri uri)
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
            if (uri == null) { return new ArgumentNullException(nameof(uri)); }
            if (!uri.IsAbsoluteUri) { return new ArgumentException($"URI must be absolute: {uri}"); }

            // Scheme check
            // Reference: https://tools.ietf.org/html/rfc3986#section-3.1
            // scheme      = ALPHA *( ALPHA / DIGIT / "+" / "-" / "." )
            if (!ReferenceScheme.IsValidScheme(uri.Scheme)) { return new ArgumentException($"URI reference scheme is invalid: {uri.Scheme}"); }

            // Authority check
            // Reference: https://tools.ietf.org/html/rfc3986#section-3.2
            //   authority   = [ userinfo "@" ] host [ ":" port ]
            if (!string.IsNullOrEmpty(uri.UserInfo)) { return new ArgumentException($"URI authority userinfo is not allowed: ${uri.UserInfo}"); }
            if (!uri.IsDefaultPort) { return new ArgumentException($"URI port is not allowed: {uri.Port}"); }

            // All paths are valid, no check required

            // Query check
            if (!string.IsNullOrEmpty(uri.Query)) { return new ArgumentException($"URI query is not allowed: {uri.Query}"); }
            // Fragment check
            if (!string.IsNullOrEmpty(uri.Fragment)) { return new ArgumentException($"URI fragment is not allowed: {uri.Fragment}"); }

            // All good!
            return null;
        }

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
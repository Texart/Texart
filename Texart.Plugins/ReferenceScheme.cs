using System;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Text.RegularExpressions;

namespace Texart.Plugins
{
    /// <summary>
    /// A <see cref="ReferenceScheme"/> is akin to URI schemes (such as <code>file:</code> and <code>http:</code>).
    /// As such, it is also case-insensitive.
    ///
    /// Note that not every valid URI scheme is a valid <code>ResourceScheme</code>. Specifically, the characters,
    /// <code>+</code>, and <code>.</code>, are not allowed. However, <code>-</code> is allowed. This may relaxed
    /// in the future.
    ///
    /// Refer to <see href="https://tools.ietf.org/html/rfc3986#section-3.1"/> for information on URI schemes
    /// (which are valid <see cref="ReferenceScheme"/>s, except for the restrictions listed above).
    /// <code>
    ///     scheme      = ALPHA *( ALPHA / DIGIT / "+" / "-" / "." )
    /// </code>
    ///
    /// Use cases:
    ///   * As a way to allow custom resolution strategies for <code>#load</code> and <code>#r</code> directives.
    ///     See <see cref="Scripting.MetadataReferenceResolverDemux"/>
    ///     and <see cref="Scripting.SourceReferenceResolverDemux"/>.
    ///   * As part of identify (or locating) objects in a plugin assembly. See <see cref="PluginResourceLocator"/>.
    /// </summary>
    /// <see cref="Uri.Scheme"/>
    public sealed class ReferenceScheme : IComparable<ReferenceScheme>, IEquatable<ReferenceScheme>
    {
        /// <summary>
        /// All valid schemes must match this regex.
        /// <see href="https://tools.ietf.org/html/rfc3986#section-3.1"/>
        /// </summary>
        private static readonly Regex SchemeRegex = new Regex(
            @"^[a-zA-Z][a-zA-Z\-0-9]*(?!\s)+$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// The underlying scheme string. This will always be lower-cased.
        /// </summary>
        public string Scheme { get; }
        /// <summary>
        /// The actual prefix of a path that represents this scheme.
        /// </summary>
        /// <see cref="Uri.SchemeDelimiter"/>
        public string SchemePrefix => $"{Scheme}{Uri.SchemeDelimiter}";

        /// <summary>
        /// Checks if the provided string can be a valid scheme. If this returns <code>false</code>,
        /// then trying to construct with this string will throw an exception.
        /// </summary>
        /// <param name="scheme">The scheme to check.</param>
        /// <returns>If this scheme is valid.</returns>
        public static bool IsValidScheme(string scheme)
        {
            if (scheme == null) { throw new ArgumentNullException(scheme); }
            return Uri.CheckSchemeName(scheme) && SchemeRegex.IsMatch(scheme);
        }

        /// <summary>
        /// Constructs a scheme from the given string. It must match <see cref="SchemeRegex"/>.
        /// </summary>
        /// <param name="scheme"></param>
        public ReferenceScheme(string scheme)
        {
            if (!IsValidScheme(scheme))
            {
                throw new ArgumentException($"{nameof(scheme)} is not valid: {scheme}");
            }

            this.Scheme = scheme.ToLower();
        }

        /// <summary>
        /// Determines whether or not the provided path matches this scheme.
        /// </summary>
        /// <param name="path">the path to check against</param>
        /// <returns>true if match, false otherwise</returns>
        public bool Matches(string path) => path.StartsWith(SchemePrefix);

        /// <summary>
        /// Returns the provided path with this scheme removed.
        /// <see cref="Matches"/> must return <code>true</code> with the provided path.
        /// </summary>
        /// <param name="path">the path to remove scheme from</param>
        /// <returns>the path without this scheme</returns>
        public string NormalizePath(string path)
        {
            Debug.Assert(this.Matches(path));
            return path.Substring(SchemePrefix.Length);
        }
        /// <summary>
        /// Returns the provided path with this scheme applied.
        /// </summary>
        /// <param name="path">the path to add scheme to</param>
        /// <returns>the path with this scheme</returns>
        public string Prefix(string path) => $"{SchemePrefix}{path}";

        /// <inheritdoc />
        public override string ToString() => Scheme;

        /// <inheritdoc />
        public int CompareTo(ReferenceScheme other) =>
            string.Compare(Scheme, other.Scheme, StringComparison.Ordinal);

        /// <inheritdoc />
        public bool Equals(ReferenceScheme other)
        {
            if (other is null) return false;
            return ReferenceEquals(this, other) || string.Equals(Scheme, other.Scheme);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is ReferenceScheme other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Scheme != null ? Scheme.GetHashCode() : 0;

        /// <summary>
        /// Compares two <see cref="ReferenceScheme"/> for equality.
        /// </summary>
        /// <param name="lhs">The left hand side of the equality.</param>
        /// <param name="rhs">The right hand side of the equality.</param>
        /// <returns>Whether the two instances refer to the same scheme or not.</returns>
        public static bool operator ==(ReferenceScheme lhs, ReferenceScheme rhs) => Equals(lhs, rhs);

        /// <summary>
        /// Compares two <see cref="ReferenceScheme"/> for inequality.
        /// </summary>
        /// <param name="lhs">The left hand side of the inequality.</param>
        /// <param name="rhs">The right hand side of the inequality.</param>
        /// <returns>Whether the two instances refer to different schemes or not.</returns>
        public static bool operator !=(ReferenceScheme lhs, ReferenceScheme rhs) => !(lhs == rhs);
    }


}
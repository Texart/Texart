using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

#nullable enable

namespace Texart.Api
{
    /// <summary>
    /// A <see cref="TxReferenceScheme"/> represents a URI scheme (such as <c>file:</c> and <c>https:</c>).
    /// As such, it is also case-insensitive.
    ///
    /// Note that not every valid URI scheme is a valid <see cref="TxReferenceScheme"/>. Specifically, the characters,
    /// <c>+</c>, and <c>.</c>, are not allowed. However, <c>-</c> is allowed. This restriction <i>may</i> be relaxed
    /// in the future.
    ///
    /// Refer to <see href="https://tools.ietf.org/html/rfc3986#section-3.1"/> for information on URI schemes
    /// (which are valid <see cref="TxReferenceScheme"/>s, except for the restrictions listed above).
    /// <code>
    /// //    scheme      = ALPHA *( ALPHA / DIGIT / "+" / "-" / "." )
    /// </code>
    ///
    /// Use cases:
    ///   * As a way to allow custom resolution strategies for <c>#load</c> and <c>#r</c> directives in Texart scripts.
    ///   * As a part of locating resources in a <see cref="ITxPlugin"/> assembly. See <see cref="TxPluginResourceLocator"/>.
    /// </summary>
    /// <seealso cref="Uri.Scheme"/>
    public sealed class TxReferenceScheme : IComparable<TxReferenceScheme>, IEquatable<TxReferenceScheme>
    {
        /// <summary>
        /// Scheme for the <c>file</c> protocol.
        /// </summary>
        public static TxReferenceScheme File => new TxReferenceScheme("file");
        /// <summary>
        /// Scheme for the <c>http</c> protocol.
        /// </summary>
        public static TxReferenceScheme Http => new TxReferenceScheme("http");
        /// <summary>
        /// Scheme for the <c>https</c> protocol.
        /// </summary>
        public static TxReferenceScheme Https => new TxReferenceScheme("https");
        /// <summary>
        /// Scheme for the <c>tx</c> protocol.
        /// </summary>
        public static TxReferenceScheme Tx => new TxReferenceScheme("tx");

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
        /// Checks if the provided string can be a valid scheme. If this returns <c>false</c>,
        /// then trying to construct with this string will throw an exception.
        /// </summary>
        /// <param name="scheme">The scheme to check.</param>
        /// <returns><c>true</c> if <c>this</c> scheme is valid, <c>false</c> otherwise.</returns>
        public static bool IsValidScheme(string scheme)
        {
            if (scheme is null) { throw new ArgumentNullException(nameof(scheme)); }
            return Uri.CheckSchemeName(scheme) && SchemeRegex.IsMatch(scheme);
        }

        /// <summary>
        /// Constructs a scheme from the given string.
        /// </summary>
        /// <param name="scheme">The case-insensitive scheme. See <see cref="Scheme"/>.</param>
        public TxReferenceScheme(string scheme)
        {
            if (!IsValidScheme(scheme))
            {
                throw new FormatException($"{nameof(scheme)} is not valid: {scheme}");
            }
            Scheme = scheme.ToLower();
        }

        /// <summary>
        /// Determines whether or not the provided path matches <c>this</c> scheme.
        /// </summary>
        /// <param name="path">The path to check against.</param>
        /// <returns><c>true</c> if match, <c>false</c> otherwise.</returns>
        public bool Matches(string path) => path.StartsWith(SchemePrefix);

        /// <summary>
        /// Returns the provided path with <c>this</c> scheme removed.
        /// <see cref="Matches"/> must return <c>true</c> with the provided path.
        /// </summary>
        /// <param name="path">The path to remove scheme from.</param>
        /// <returns><paramref name="path"/> without <c>this</c> scheme.</returns>
        public string NormalizePath(string path)
        {
            Debug.Assert(Matches(path));
            return path.Substring(SchemePrefix.Length);
        }
        /// <summary>
        /// Returns the provided path with <c>this</c> scheme applied.
        /// </summary>
        /// <param name="path">the path to add scheme to.</param>
        /// <returns><paramref name="path"/> with <c>this</c> scheme added.</returns>
        public string Prefix(string path) => $"{SchemePrefix}{path}";

        /// <inheritdoc/>
        public override string ToString() => Scheme;

        /// <inheritdoc/>
        public int CompareTo(TxReferenceScheme other) =>
            string.Compare(Scheme, other.Scheme, StringComparison.Ordinal);

        /// <inheritdoc/>
        public bool Equals(TxReferenceScheme other)
        {
            if (other is null) return false;
            return ReferenceEquals(this, other) || string.Equals(Scheme, other.Scheme);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is TxReferenceScheme other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => Scheme != null ? Scheme.GetHashCode() : 0;

        /// <summary>
        /// Compares two <see cref="TxReferenceScheme"/> for equality.
        /// </summary>
        /// <param name="lhs">The left hand side of the equality.</param>
        /// <param name="rhs">The right hand side of the equality.</param>
        /// <returns>Whether the two instances refer to the same scheme or not.</returns>
        public static bool operator ==(TxReferenceScheme lhs, TxReferenceScheme rhs) => Equals(lhs, rhs);

        /// <summary>
        /// Compares two <see cref="TxReferenceScheme"/> for inequality.
        /// </summary>
        /// <param name="lhs">The left hand side of the inequality.</param>
        /// <param name="rhs">The right hand side of the inequality.</param>
        /// <returns>Whether the two instances refer to different schemes or not.</returns>
        public static bool operator !=(TxReferenceScheme lhs, TxReferenceScheme rhs) => !(lhs == rhs);

        /// <summary>
        /// A <see cref="FormatException"/> is thrown when an attempt is made to create <see cref="TxReferenceScheme"/>
        /// with an invalid string.
        /// </summary>
        public sealed class FormatException : System.FormatException
        {
            /// <summary>
            /// Creates an exception with <see cref="Exception.Message"/> set to <paramref name="message"/>.
            /// </summary>
            /// <param name="message">The exception message.</param>
            public FormatException(string message) : base(message) { }
        }
    }


}
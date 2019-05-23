using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Texart.Plugins.Scripting
{
    public readonly struct ReferenceScheme : IComparable<ReferenceScheme>, IEquatable<ReferenceScheme>
    {
        /// <summary>
        /// All valid schemes must match this regex
        /// </summary>
        private static readonly Regex SchemeRegex = new Regex(
            @"^[a-zA-Z][a-zA-Z\-0-9]*(?!\s)+$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// The underlying string.
        /// </summary>
        public string Scheme { get; }
        /// <summary>
        /// The actual prefix of a path that represents this scheme.
        /// </summary>
        public string SchemePrefix => $"{Scheme}:";

        /// <summary>
        /// Constructs a scheme from the given string. It must match <see cref="SchemeRegex"/>.
        /// </summary>
        /// <param name="scheme"></param>
        public ReferenceScheme(string scheme)
        {
            if (scheme == null || !SchemeRegex.IsMatch(scheme))
            {
                throw new ArgumentException($"{nameof(scheme)} is not valid: {scheme}");
            }

            this.Scheme = scheme;
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
        public bool Equals(ReferenceScheme other) => Scheme == other.Scheme;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is ReferenceScheme scheme && Equals(scheme);

        /// <inheritdoc />
        public override int GetHashCode() => Scheme.GetHashCode();
    }
}
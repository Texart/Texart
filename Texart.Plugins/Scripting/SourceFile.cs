using System;
using System.IO;
using System.Text;

namespace Texart.Plugins.Scripting
{
    /// <summary>
    /// An object that encapsulates a file path and the text in that file.
    /// </summary>
    public sealed class SourceFile : IEquatable<SourceFile>
    {
        /// <summary>
        /// The path of the source file. This should be an absolute path, since Roslyn usually demands it.
        /// </summary>
        public string FilePath { get; }
        /// <summary>
        /// The text in the source file. This is just the file contents, properly decoded.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Creates an instance with the provided <see cref="FilePath"/> and <see cref="Text"/> properties.
        /// </summary>
        /// <param name="filePath">The path of the source file.</param>
        /// <param name="text">The text in the source file.</param>
        private SourceFile(string filePath, string text)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        /// Creates an instance of <see cref="SourceFile"/> by open the file at the given path.
        /// The returned instance's <see cref="FilePath"/> is guaranteed to be an absolute path.
        /// </summary>
        /// <param name="filePath">The file path to load text from.</param>
        /// <param name="encoding">
        ///     The encoding to use to read the file contents.
        ///     If this is <c>null</c>, then <see cref="Encoding.UTF8"/> is used.
        /// </param>
        /// <returns>A <c>SourceFile</c> instance.</returns>
        public static SourceFile Load(string filePath, Encoding encoding = null)
        {
            var absolutePath = Path.GetFullPath(filePath);

            using (var file = File.OpenRead(absolutePath))
            using (var reader = new StreamReader(file, encoding ?? Encoding.UTF8))
            {
                return new SourceFile(absolutePath, reader.ReadToEnd());
            }
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(FilePath, Text);

        /// <inheritdoc />
        public bool Equals(SourceFile other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(FilePath, other.FilePath) && string.Equals(Text, other.Text);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is SourceFile other && Equals(other);
    }
}
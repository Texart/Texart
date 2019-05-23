using System;

namespace Texart.Plugins.Scripting
{
    /// <summary>
    /// A tuple of file path and the source code in that file.
    /// </summary>
    public sealed class SourceFile : IEquatable<SourceFile>
    {
        public string FilePath { get; }
        public string Code { get; }

        public SourceFile(string filePath, string code)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        public static SourceFile FromFile(string filePath)
        {
            // TODO: implement this
            throw new NotImplementedException();
        }

        public override int GetHashCode() => HashCode.Combine(FilePath, Code);

        public bool Equals(SourceFile other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(FilePath, other.FilePath) && string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is SourceFile other && Equals(other);
    }
}
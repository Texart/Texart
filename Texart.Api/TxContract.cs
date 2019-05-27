using System;

namespace Texart.Api
{
    /// <summary>
    /// Helpers methods around code validation.
    /// </summary>
    public static class TxContract
    {
        /// <summary>
        /// Asserts that the given value is not <c>null</c>.
        /// If the given value is <c>null</c>, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <returns>The provided value if it's not <c>null</c>.</returns>
        public static T NonNull<T>(T value)
        {
            if (value == null) { throw new ArgumentNullException($"TxContract: {nameof(value)} was null!"); }
            return value;
        }
    }
}
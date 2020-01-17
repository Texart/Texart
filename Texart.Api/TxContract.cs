using System;
using System.Runtime.CompilerServices;

#nullable enable

namespace Texart.Api
{
    /// <summary>
    /// Helpers methods around code validation.
    /// </summary>
    /// <seealso cref="System.Diagnostics.Contracts.Contract"/>
    public static class TxContract
    {
        /// <summary>
        /// Asserts that the given value is not <c>null</c>.
        /// If the given value is <c>null</c>, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="valueExpr">Caller argument expression of <paramref name="value"/></param>
        /// <returns>The provided value if it's not <c>null</c>.</returns>
        public static T NonNull<T>(T value, [CallerArgumentExpression("value")] string? valueExpr = null)
        {
            if (value is null) { throw new ArgumentNullException($"TxContract: {valueExpr ?? nameof(value)} was null!"); }
            return value;
        }
    }
}
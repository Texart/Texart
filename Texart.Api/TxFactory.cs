namespace Texart.Api
{
    /// <summary>
    /// A factory function taking exactly one input argument.
    /// </summary>
    /// <typeparam name="TResult">The constructed type.</typeparam>
    /// <typeparam name="TInput">The argument type.</typeparam>
    /// <param name="input">The input argument.</param>
    /// <returns>Constructed result.</returns>
    public delegate TResult TxFactory<out TResult, in TInput>(TInput input);
}

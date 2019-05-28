using System.Collections.Generic;

namespace Texart.Api
{
    /// <summary>
    /// A plugin represents a collection of named <see cref="ITextBitmapGenerator"/> and <see cref="ITextBitmapRenderer"/>.
    /// The plugin abstraction allows sharing these types across assembly boundaries.
    ///
    /// Using types defined in the following contexts are unified by this interface:
    ///   * Within Texart code
    ///   * Within a separately compiled assembly
    ///   * Within a .tx.csx file that will be interpreted at runtime
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Available names of <see cref="ITextBitmapGenerator"/>. Every name listed here
        /// must be valid when calling <see cref="LookupGenerator"/>.
        /// </summary>
        IEnumerable<string> AvailableGenerators { get; }

        /// <summary>
        /// Returns a factory function that constructs an <see cref="ITextBitmapGenerator"/> identified by
        /// <paramref name="name"/>. The factory function accepts a <see cref="TxArguments"/> which can be used
        /// configure the instance. The format of the arguments is implementation-defined.
        /// </summary>
        /// <param name="name">
        ///     The name to look up. Check <see cref="AvailableGenerators"/>.
        ///     If a plugin exports a "default" type, use <c>null</c>.
        /// </param>
        /// <returns>Factory function for <see cref="ITextBitmapGenerator"/></returns>
        TxFactory<ITextBitmapGenerator, TxArguments> LookupGenerator(string name);

        /// <summary>
        /// Available names of <see cref="ITextBitmapRenderer"/>. Every name listed here
        /// must be valid when calling <see cref="LookupRenderer"/>.
        /// </summary>
        IEnumerable<string> AvailableRenderers { get; }

        /// <summary>
        /// Returns a factory function that constructs an <see cref="ITextBitmapRenderer"/> identified by
        /// <paramref name="name"/>. The factory function accepts a <see cref="TxArguments"/> which can be used
        /// configure the instance. The format of the arguments is implementation-defined.
        /// </summary>
        /// <param name="name">
        ///     The name to look up. Check <see cref="AvailableRenderers"/>.
        ///     If a plugin exports a "default" type, use <c>null</c>.
        /// </param>
        /// <returns>Factory function for <see cref="ITextBitmapRenderer"/></returns>
        TxFactory<ITextBitmapRenderer, TxArguments> LookupRenderer(string name);
    }
}
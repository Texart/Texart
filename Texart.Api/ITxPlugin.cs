using System.Collections.Generic;

namespace Texart.Api
{
    /// <summary>
    /// A plugin represents a collection of named <see cref="ITxTextBitmapGenerator"/> and <see cref="ITxTextBitmapRenderer"/>.
    /// The plugin abstraction allows sharing these types across assembly boundaries.
    ///
    /// Using types defined in the following contexts are unified by this interface:
    ///   * Within Texart code
    ///   * Within a separately compiled assembly
    ///   * Within a .tx.csx file that will be interpreted at runtime
    /// </summary>
    public interface ITxPlugin
    {
        /// <summary>
        /// Available names of <see cref="ITxTextBitmapGenerator"/>. Every name listed here must be valid when calling
        /// <see cref="LookupGenerator"/>.
        /// </summary>
        IEnumerable<string> AvailableGenerators { get; }

        /// <summary>
        /// Returns a factory function that constructs an <see cref="ITxTextBitmapGenerator"/> identified by
        /// <paramref name="name"/>, or a locator which represents a redirects to another lookup
        /// (<see cref="TxPluginResourceLocator"/> or <see cref="TxPluginResourceLocator.Relative"/>).
        /// </summary>
        /// <param name="name">
        ///     The name to look up. This name should appear in <see cref="AvailableGenerators"/> but not required.
        ///     If a plugin exports a "default" type, then the type should be available as <c>null</c>.
        /// </param>
        /// <returns>A resource specification for <see cref="ITxTextBitmapGenerator"/></returns>
        /// <seealso cref="TxPluginResource.OfFactory{T}"/>
        /// <seealso cref="TxPluginResource.OfLocator{T}(TxPluginResourceLocator)"/>
        /// <seealso cref="TxPluginResource.OfLocator{T}(TxPluginResourceLocator.Relative)"/>
        TxPluginResource<ITxTextBitmapGenerator> LookupGenerator(string name);

        /// <summary>
        /// Available names of <see cref="ITxTextBitmapRenderer"/>. Every name listed here must be valid when calling
        /// <see cref="LookupRenderer"/>.
        /// </summary>
        IEnumerable<string> AvailableRenderers { get; }

        /// <summary>
        /// Returns a factory function that constructs an <see cref="ITxTextBitmapRenderer"/> identified by
        /// <paramref name="name"/>, or a locator which represents a redirects to another lookup
        /// (<see cref="TxPluginResourceLocator"/> or <see cref="TxPluginResourceLocator.Relative"/>).
        /// </summary>
        /// <param name="name">
        ///     The name to look up. This name should appear in <see cref="AvailableRenderers"/> but not required.
        ///     If a plugin exports a "default" type, then the type should be available as <c>null</c>.
        /// </param>
        /// <returns>A resource specification for <see cref="ITxTextBitmapRenderer"/></returns>
        /// <seealso cref="TxPluginResource.OfFactory{T}"/>
        /// <seealso cref="TxPluginResource.OfLocator{T}(TxPluginResourceLocator)"/>
        /// <seealso cref="TxPluginResource.OfLocator{T}(TxPluginResourceLocator.Relative)"/>
        TxPluginResource<ITxTextBitmapRenderer> LookupRenderer(string name);
    }
}
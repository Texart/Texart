using System.Collections.Generic;

#nullable enable

namespace Texart.Api
{
    using Locator = TxPluginResourceLocator;
    using RelativeLocator = TxPluginResourceLocator.RelativeLocator;

    /// <summary>
    /// A plugin represents a collection of named <see cref="ITxTextBitmapGenerator"/> and <see cref="ITxTextBitmapRenderer"/>.
    /// The plugin abstraction allows sharing these types across assembly boundaries.
    ///
    /// <see cref="ITxPlugin"/> unifies sharing "resources" (like algorithms) in the following contexts:
    ///   * Within Texart code.
    ///   * Within a separately compiled assembly, built against <c>Texart.Api</c>.
    ///   * Within a <c>.tx.csx</c> script file that will be interpreted by the Texart runtime.
    /// </summary>
    public interface ITxPlugin
    {
        /// <summary>
        /// Available names of <see cref="ITxTextBitmapGenerator"/>. Every name listed here must be valid when calling
        /// <see cref="LookupGenerator"/>.
        /// </summary>
        IEnumerable<RelativeLocator> AvailableGenerators { get; }
        /// <summary>
        /// Returns a factory function that constructs an <see cref="ITxTextBitmapGenerator"/> identified by
        /// <paramref name="locator"/>, or a "redirect" locator which represents another lookup
        /// (<see cref="TxPluginResource.Redirect{T}"/> and <see cref="Locator"/>).
        /// </summary>
        /// <param name="locator">
        ///     The resource to look up. This identity <i>should</i> appear in <see cref="AvailableGenerators"/>
        ///     but is not required to. If a plugin exports a "default" type, then the type should be available
        ///     as an empty <see cref="Locator.ResourcePath"/>.
        /// </param>
        /// <returns>A resource specification for <see cref="ITxTextBitmapGenerator"/></returns>
        /// <seealso cref="TxPluginResource.OfFactory{T}"/>
        /// <seealso cref="TxPluginResource.OfGeneratorFactory{T}"/>
        /// <seealso cref="TxPluginResource.Redirect{T}"/>
        /// <seealso cref="TxPluginResource.RedirectGenerator"/>
        TxPluginResource<ITxTextBitmapGenerator> LookupGenerator(Locator locator);

        /// <summary>
        /// Available names of <see cref="ITxTextBitmapRenderer"/>. Every name listed here must be valid when calling
        /// <see cref="LookupRenderer"/>.
        /// </summary>
        IEnumerable<RelativeLocator> AvailableRenderers { get; }
        /// <summary>
        /// Returns a factory function that constructs an <see cref="ITxTextBitmapRenderer"/> identified by
        /// <paramref name="locator"/>, or a "redirect" locator which represents another lookup
        /// (<see cref="TxPluginResource.Redirect{T}"/> and <see cref="Locator"/>).
        /// </summary>
        /// <param name="locator">
        ///     The resource to look up. This identity <i>should</i> appear in <see cref="AvailableRenderers"/>
        ///     but is not required to. If a plugin exports a "default" type, then the type should be available
        ///     as an empty <see cref="Locator.ResourcePath"/>.
        /// </param>
        /// <returns>A resource specification for <see cref="ITxTextBitmapRenderer"/></returns>
        /// <seealso cref="TxPluginResource.OfFactory{T}"/>
        /// <seealso cref="TxPluginResource.OfRendererFactory{T}"/>
        /// <seealso cref="TxPluginResource.Redirect{T}"/>
        /// <seealso cref="TxPluginResource.RedirectRenderer"/>
        TxPluginResource<ITxTextBitmapRenderer> LookupRenderer(Locator locator);

        /// <summary>
        /// Available names of packages to look up. Every name listed here must be valid when calling
        /// <see cref="LookupGenerator"/> and <see cref="LookupRenderer"/>.
        /// </summary>
        IEnumerable<RelativeLocator> AvailablePackages { get; }

        /// <summary>
        /// Returns a pair of <see cref="RelativeLocator"/>s that identify the <see cref="ITxTextBitmapGenerator"/>
        /// and <see cref="ITxTextBitmapRenderer"/> for the package named by <paramref name="locator"/>.
        /// </summary>
        /// <param name="locator">
        ///     The package to look up. This identity <i>should</i> appear in <see cref="AvailablePackages"/>
        ///     but is not required to. If a plugin exports a "default" type, then the type should be available
        ///     as an empty <see cref="Locator.ResourcePath"/>.
        /// </param>
        /// <returns>A pair of <see cref="RelativeLocator"/>s that identify the resources in the package.</returns>
        (RelativeLocator generator, RelativeLocator renderer) LookupPackage(Locator locator);

        /// <summary>
        /// Prints help information for <c>this</c> into <paramref name="console"/>.
        /// </summary>
        /// <param name="console">The output console to print help to.</param>
        void PrintHelp(ITxConsole console);
        /// <summary>
        /// Prints help information for a resource represented by <paramref name="resourceKind"/> and
        /// identified by <paramref name="locator"/>.
        /// </summary>
        /// <param name="console">The output console to print help to.</param>
        /// <param name="resourceKind">The kind of resource to get help for.</param>
        /// <param name="locator">The identity of the resource to get help for.</param>
        void PrintHelp(ITxConsole console, TxPluginResourceKind resourceKind, Locator locator);
    }
}
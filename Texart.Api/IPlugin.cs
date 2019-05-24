using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Texart.Api
{
    /// <summary>
    /// A plugin represents a collection of named <see cref="ITextBitmapGenerator"/> and <see cref="ITextBitmapRenderer"/>.
    /// The plugin abstraction allows sharing these types across assembly boundaries.
    ///
    /// A JSON value may be passed to the plugin if they contained types require additional arguments. The format of this
    /// JSON value is implementation-defined.
    ///
    /// Using types defined in the following contexts are unified by this interface:
    ///   * Within Texart code
    ///   * Within a separately compiled assembly (this is what a "real plugin" is)
    ///   * Within a .csx file that will be interpreted at runtime
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Available names of <see cref="ITextBitmapGenerator"/>. Every name listed here
        /// must be valid when calling <see cref="LookupGenerator"/>.
        /// </summary>
        IEnumerable<string> AvailableGenerators { get; }

        /// <summary>
        /// Returns a factory function that constructs an <see cref="ITextBitmapGenerator"/>
        /// identified by the given named. The factory function also accepts a JSON value that
        /// can be used to pass arguments required to create the instance. The format of this
        /// JSON value is implementation-defined.
        /// </summary>
        /// <param name="name">
        ///     The name to look up. Check <see cref="AvailableGenerators"/>.
        ///     If a plugin exports a "default" type, use <code>null</code>.
        /// </param>
        /// <returns>Factory function for <see cref="ITextBitmapGenerator"/></returns>
        Factory<ITextBitmapGenerator, Lazy<JToken>> LookupGenerator(string name);

        /// <summary>
        /// Available names of <see cref="ITextBitmapRenderer"/>. Every name listed here
        /// must be valid when calling <see cref="LookupRenderer"/>.
        /// </summary>
        IEnumerable<string> AvailableRenderers { get; }

        /// <summary>
        /// Returns a factory function that constructs an <see cref="ITextBitmapRenderer"/>
        /// identified by the given named. The factory function also accepts a JSON value that
        /// can be used to pass arguments required to create the instance. The format of this
        /// JSON value is implementation-defined.
        /// </summary>
        /// <param name="name">
        ///     The name to look up. Check <see cref="AvailableRenderers"/>.
        ///     If a plugin exports a "default" type, use <code>null</code>.
        /// </param>
        /// <returns>Factory function for <see cref="ITextBitmapRenderer"/></returns>
        Factory<ITextBitmapRenderer, Lazy<JToken>> LookupRenderer(string name);
    }
}
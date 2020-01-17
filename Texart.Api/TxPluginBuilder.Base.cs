using System;
using System.Collections.Generic;

#nullable enable

namespace Texart.Api
{
    using Locator = TxPluginResourceLocator;
    using RelativeLocator = TxPluginResourceLocator.RelativeLocator;

    public sealed partial class TxPluginBuilder
    {
        /// <summary>
        /// A simple base class for a <see cref="ITxPlugin"/> that is described using <see cref="TxPluginBuilder"/>.
        /// </summary>
        /// <example>
        /// [TxPlugin]
        /// public sealed class Plugin : TxPluginBuilder.Base, ITxPlugin
        /// {
        ///     private static TxPluginBuilder BuilderDescription => new TxPluginBuilder()
        ///         .AddGenerator(typeof(MyGenerator), MyGeneratorFactory)
        ///         .AddRenderer(typeof(MyRenderer), MyRendererFactory)
        ///         .SetHelp("some help for this plugin");
        ///
        ///     public Plugin() : base(BuilderDescription) { }
        /// }
        /// </example>
        public abstract class Base : ITxPlugin
        {
            /// <summary>
            /// Initializes <c>this</c> using the current state of <paramref name="builder"/>.
            /// </summary>
            /// <param name="builder">The plugin builder description.</param>
            protected Base(TxPluginBuilder builder)
            {
                if (builder is null)
                {
                    throw new ArgumentNullException(nameof(builder));
                }
                _plugin = builder.CreatePlugin();
            }

            /// <summary>
            /// The internal underlying <see cref="ITxPlugin"/> instance from <see cref="TxPluginBuilder"/>.
            /// </summary>
            private readonly ITxPlugin _plugin;

            /// <inheritdoc/>
            public IEnumerable<RelativeLocator> AvailableGenerators => _plugin.AvailableGenerators;

            /// <inheritdoc/>
            public TxPluginResource<ITxTextBitmapGenerator> LookupGenerator(Locator locator) =>
                _plugin.LookupGenerator(locator);

            /// <inheritdoc/>
            public IEnumerable<RelativeLocator> AvailableRenderers => _plugin.AvailableRenderers;

            /// <inheritdoc/>
            public TxPluginResource<ITxTextBitmapRenderer> LookupRenderer(Locator locator) =>
                _plugin.LookupRenderer(locator);

            /// <inheritdoc/>
            public IEnumerable<RelativeLocator> AvailablePackages => _plugin.AvailablePackages;

            /// <inheritdoc/>
            public (RelativeLocator generator, RelativeLocator renderer) LookupPackage(Locator locator) =>
                _plugin.LookupPackage(locator);

            /// <inheritdoc/>
            public void PrintHelp(ITxConsole console) => _plugin.PrintHelp(console);

            /// <inheritdoc/>
            public void PrintHelp(ITxConsole console, TxPluginResourceKind resourceKind, Locator locator) =>
                _plugin.PrintHelp(console, resourceKind, locator);
        }
    }
}
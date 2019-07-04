using System.Collections.Generic;

#nullable enable

namespace Texart.Api
{
    using Locator = TxPluginResourceLocator;
    using RelativeLocator = TxPluginResourceLocator.RelativeLocator;

    public sealed partial class TxPluginBuilder
    {
        public abstract class Base : ITxPlugin
        {
            protected Base(TxPluginBuilder builder)
            {
                _plugin = builder.CreatePlugin();
            }

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
            public void PrintHelp(ITxConsole console) => _plugin.PrintHelp(console);

            /// <inheritdoc/>
            public void PrintHelp(ITxConsole console, TxPluginResourceKind resourceKind, Locator locator) =>
                _plugin.PrintHelp(console, resourceKind, locator);
        }
    }
}
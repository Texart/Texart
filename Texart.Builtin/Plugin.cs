using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Texart.Builtin.Generators;
using Texart.Builtin.Renderers;
using Texart.Api;

namespace Texart.Builtin
{
    using Locator = TxPluginResourceLocator;
    using RelativeLocator = TxPluginResourceLocator.RelativeLocator;

    /// <summary>
    /// The plugin implementation for the built in <see cref="ITxTextBitmapGenerator"/> and <see cref="ITxTextBitmapRenderer"/> types.
    /// </summary>
    public sealed class Plugin : ITxPlugin
    {
        /// <summary>
        /// Mapping of names to <see cref="ITxTextBitmapGenerator"/> factory functions.
        /// </summary>
        private readonly ImmutableDictionary<RelativeLocator, TxPluginResource<ITxTextBitmapGenerator>> _generators =
            new Dictionary<RelativeLocator, TxPluginResource<ITxTextBitmapGenerator>>
            {
                {
                    Locator.OfRelativeResource(typeof(BrightnessBasedBitmapGenerator).Name),
                    TxPluginResource.OfGeneratorFactory(BrightnessBasedBitmapGenerator.Create)
                }
            }.ToImmutableDictionary();

        /// <summary>
        /// The default generator when the given name is <c>null</c>.
        /// </summary>
        private readonly TxPluginResource<ITxTextBitmapGenerator> _defaultGenerator =
            TxPluginResource.OfGeneratorFactory(BrightnessBasedBitmapGenerator.Create);

        /// <summary>
        /// Mapping of names to <see cref="ITxTextBitmapRenderer"/> factory functions.
        /// </summary>
        private readonly ImmutableDictionary<RelativeLocator, TxPluginResource<ITxTextBitmapRenderer>> _renderers =
            new Dictionary<RelativeLocator, TxPluginResource<ITxTextBitmapRenderer>>
            {
                {
                    TxPluginResourceLocator.OfRelativeResource(typeof(StringBitmapRenderer).Name),
                    TxPluginResource.OfRendererFactory(StringBitmapRenderer.Create)
                },
                {
                    TxPluginResourceLocator.OfRelativeResource(typeof(FontBitmapRenderer).Name),
                    TxPluginResource.OfRendererFactory(FontBitmapRenderer.Create)
                }
            }.ToImmutableDictionary();

        /// <summary>
        /// The default renderer when the given name is <c>null</c>.
        /// </summary>
        private readonly TxPluginResource<ITxTextBitmapRenderer> _defaultRenderer =
            TxPluginResource.OfRendererFactory(FontBitmapRenderer.Create);

        /// <inheritdoc cref="ITxPlugin.AvailableGenerators"/>
        IEnumerable<RelativeLocator> ITxPlugin.AvailableGenerators => _generators.Keys;

        /// <inheritdoc cref="ITxPlugin.LookupGenerator" />
        TxPluginResource<ITxTextBitmapGenerator> ITxPlugin.LookupGenerator(Locator locator)
        {
            if (locator.ResourcePath == string.Empty)
            {
                return _defaultGenerator;
            }
            if (_generators.TryGetValue(locator.RelativeResource, out var factory))
            {
                return factory;
            }
            throw new ArgumentException($"No {nameof(ITxTextBitmapGenerator)} named '{locator.RelativeResource}' exists.");
        }

        /// <inheritdoc cref="ITxPlugin.AvailableRenderers"/>
        IEnumerable<RelativeLocator> ITxPlugin.AvailableRenderers => _renderers.Keys;

        /// <inheritdoc cref="ITxPlugin.LookupRenderer"/>
        TxPluginResource<ITxTextBitmapRenderer> ITxPlugin.LookupRenderer(Locator locator)
        {
            if (locator.ResourcePath == string.Empty)
            {
                return _defaultRenderer;
            }
            if (_renderers.TryGetValue(locator.RelativeResource, out var factory))
            {
                return factory;
            }
            throw new ArgumentException($"No {nameof(ITxTextBitmapRenderer)} named '{locator.RelativeResource}' exists.");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Texart.Builtin.Generators;
using Texart.Builtin.Renderers;
using Texart.Api;

namespace Texart.Builtin
{
    /// <summary>
    /// The plugin implementation for the built in <see cref="ITxTextBitmapGenerator"/> and <see cref="ITxTextBitmapRenderer"/> types.
     /// </summary>
    public sealed class Plugin : ITxPlugin
    {
        /// <summary>
        /// Mapping of names to <see cref="ITxTextBitmapGenerator"/> factory functions.
        /// </summary>
        private readonly ImmutableDictionary<string, TxPluginResource<ITxTextBitmapGenerator>> _generators =
            new Dictionary<string, TxPluginResource<ITxTextBitmapGenerator>>
            {
                { typeof(BrightnessBasedBitmapGenerator).Name, TxPluginResource.OfGeneratorFactory(BrightnessBasedBitmapGenerator.Create) }
            }.ToImmutableDictionary();

        /// <summary>
        /// The default generator when the given name is <c>null</c>.
        /// </summary>
        private readonly TxPluginResource<ITxTextBitmapGenerator> _defaultGenerator =
            TxPluginResource.OfGeneratorFactory(BrightnessBasedBitmapGenerator.Create);

        /// <summary>
        /// Mapping of names to <see cref="ITxTextBitmapRenderer"/> factory functions.
        /// </summary>
        private readonly ImmutableDictionary<string, TxPluginResource<ITxTextBitmapRenderer>> _renderers =
            new Dictionary<string, TxPluginResource<ITxTextBitmapRenderer>>
            {
                { typeof(StringBitmapRenderer).Name, TxPluginResource.OfRendererFactory(StringBitmapRenderer.Create) },
                { typeof(FontBitmapRenderer).Name, TxPluginResource.OfRendererFactory(FontBitmapRenderer.Create) }
            }.ToImmutableDictionary();

        /// <summary>
        /// The default renderer when the given name is <c>null</c>.
        /// </summary>
        private readonly TxPluginResource<ITxTextBitmapRenderer> _defaultRenderer =
            TxPluginResource.OfRendererFactory(FontBitmapRenderer.Create);

        /// <inheritdoc cref="ITxPlugin.AvailableGenerators"/>
        IEnumerable<string> ITxPlugin.AvailableGenerators => _generators.Keys;

        /// <inheritdoc />
        TxPluginResource<ITxTextBitmapGenerator> ITxPlugin.LookupGenerator(string name)
        {
            if (name == null)
            {
                return _defaultGenerator;
            }
            if (_generators.TryGetValue(name, out var factory))
            {
                return factory;
            }
            throw new ArgumentException($"No {nameof(ITxTextBitmapGenerator)} named '{name}' exists.");
        }

        /// <inheritdoc cref="ITxPlugin.AvailableRenderers"/>
        IEnumerable<string> ITxPlugin.AvailableRenderers => _renderers.Keys;

        /// <inheritdoc cref="ITxPlugin.LookupRenderer"/>
        TxPluginResource<ITxTextBitmapRenderer> ITxPlugin.LookupRenderer(string name)
        {
            if (name == null)
            {
                return _defaultRenderer;
            }
            if (_renderers.TryGetValue(name, out var factory))
            {
                return factory;
            }
            throw new ArgumentException($"No {nameof(ITxTextBitmapRenderer)} named '{name}' exists.");
        }
    }
}
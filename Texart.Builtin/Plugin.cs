using System;
using System.Collections.Generic;
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
        private readonly IDictionary<string, TxFactory<ITxTextBitmapGenerator, TxArguments>> _generators =
            new Dictionary<string, TxFactory<ITxTextBitmapGenerator, TxArguments>>
            {
                { typeof(BrightnessBasedBitmapGenerator).Name, BrightnessBasedBitmapGenerator.Create }
            };
        /// <summary>
        /// The default generator when the given name is <c>null</c>.
        /// </summary>
        private readonly TxFactory<ITxTextBitmapGenerator, TxArguments> _defaultGenerator = BrightnessBasedBitmapGenerator.Create;

        /// <summary>
        /// Mapping of names to <see cref="ITxTextBitmapRenderer"/> factory functions.
        /// </summary>
        private readonly IDictionary<string, TxFactory<ITxTextBitmapRenderer, TxArguments>> _renderers =
            new Dictionary<string, TxFactory<ITxTextBitmapRenderer, TxArguments>>
            {
                { typeof(StringBitmapRenderer).Name, StringBitmapRenderer.Create },
                { typeof(FontBitmapRenderer).Name, FontBitmapRenderer.Create },
            };
        /// <summary>
        /// The default renderer when the given name is <c>null</c>.
        /// </summary>
        private readonly TxFactory<ITxTextBitmapRenderer, TxArguments> _defaultRenderer = FontBitmapRenderer.Create;

        /// <inheritdoc />
        public IEnumerable<string> AvailableGenerators => _generators.Keys;

        /// <inheritdoc />
        public TxFactory<ITxTextBitmapGenerator, TxArguments> LookupGenerator(string name)
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

        /// <inheritdoc />
        public IEnumerable<string> AvailableRenderers => _renderers.Keys;

        /// <inheritdoc />
        public TxFactory<ITxTextBitmapRenderer, TxArguments> LookupRenderer(string name)
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
using System;
using System.Collections.Generic;
using Texart.Builtin.Generators;
using Texart.Builtin.Renderers;
using Texart.Api;

namespace Texart.Builtin
{
    /// <summary>
    /// The plugin implementation for the built in <see cref="ITextBitmapGenerator"/> and <see cref="ITextBitmapRenderer"/> types.
     /// </summary>
    public sealed class Plugin : IPlugin
    {
        /// <summary>
        /// Mapping of names to <see cref="ITextBitmapGenerator"/> factory functions.
        /// </summary>
        private readonly IDictionary<string, TxFactory<ITextBitmapGenerator, TxArguments>> _generators =
            new Dictionary<string, TxFactory<ITextBitmapGenerator, TxArguments>>
            {
                { typeof(BrightnessBasedBitmapGenerator).Name, BrightnessBasedBitmapGenerator.Create }
            };
        /// <summary>
        /// The default generator when the given name is <c>null</c>.
        /// </summary>
        private readonly TxFactory<ITextBitmapGenerator, TxArguments> _defaultGenerator = BrightnessBasedBitmapGenerator.Create;

        /// <summary>
        /// Mapping of names to <see cref="ITextBitmapRenderer"/> factory functions.
        /// </summary>
        private readonly IDictionary<string, TxFactory<ITextBitmapRenderer, TxArguments>> _renderers =
            new Dictionary<string, TxFactory<ITextBitmapRenderer, TxArguments>>
            {
                { typeof(StringBitmapRenderer).Name, StringBitmapRenderer.Create },
                { typeof(FontBitmapRenderer).Name, FontBitmapRenderer.Create },
            };
        /// <summary>
        /// The default renderer when the given name is <c>null</c>.
        /// </summary>
        private readonly TxFactory<ITextBitmapRenderer, TxArguments> _defaultRenderer = FontBitmapRenderer.Create;

        /// <inheritdoc />
        public IEnumerable<string> AvailableGenerators => _generators.Keys;

        /// <inheritdoc />
        public TxFactory<ITextBitmapGenerator, TxArguments> LookupGenerator(string name)
        {
            if (name == null)
            {
                return _defaultGenerator;
            }
            if (_generators.TryGetValue(name, out var factory))
            {
                return factory;
            }
            throw new ArgumentException($"No {nameof(ITextBitmapGenerator)} named '{name}' exists.");
        }

        /// <inheritdoc />
        public IEnumerable<string> AvailableRenderers => _renderers.Keys;

        /// <inheritdoc />
        public TxFactory<ITextBitmapRenderer, TxArguments> LookupRenderer(string name)
        {
            if (name == null)
            {
                return _defaultRenderer;
            }
            if (_renderers.TryGetValue(name, out var factory))
            {
                return factory;
            }
            throw new ArgumentException($"No {nameof(ITextBitmapRenderer)} named '{name}' exists.");
        }
    }
}
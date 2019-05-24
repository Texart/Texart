﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
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
        private readonly IDictionary<string, Factory<ITextBitmapGenerator, Lazy<JToken>>> _generators =
            new Dictionary<string, Factory<ITextBitmapGenerator, Lazy<JToken>>>
            {
                { typeof(BrightnessBasedBitmapGenerator).Name, BrightnessBasedBitmapGenerator.Create },
            };
        /// <summary>
        /// The default generator when the given name is <code>null</code>.
        /// </summary>
        private readonly Factory<ITextBitmapGenerator, Lazy<JToken>> _defaultGenerator = BrightnessBasedBitmapGenerator.Create;

        /// <summary>
        /// Mapping of names to <see cref="ITextBitmapRenderer"/> factory functions.
        /// </summary>
        private readonly IDictionary<string, Factory<ITextBitmapRenderer, Lazy<JToken>>> _renderers =
            new Dictionary<string, Factory<ITextBitmapRenderer, Lazy<JToken>>>
            {
                { typeof(StringBitmapRenderer).Name, StringBitmapRenderer.Create },
                { typeof(FontBitmapRenderer).Name, FontBitmapRenderer.Create },
            };
        /// <summary>
        /// The default renderer when the given name is <code>null</code>.
        /// </summary>
        private readonly Factory<ITextBitmapRenderer, Lazy<JToken>> _defaultRenderer = FontBitmapRenderer.Create;

        /// <inheritdoc />
        public IEnumerable<string> AvailableGenerators => _generators.Keys;

        /// <inheritdoc />
        public Factory<ITextBitmapGenerator, Lazy<JToken>> LookupGenerator(string name)
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
        public Factory<ITextBitmapRenderer, Lazy<JToken>> LookupRenderer(string name)
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
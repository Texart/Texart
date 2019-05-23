using System;
using System.Collections.Generic;
using Texart.Builtin.Generators;
using Texart.Builtin.Renderers;
using Texart.Interface;
using Texart.Plugins;

namespace Texart.Builtin
{
    public sealed class Plugin : IPlugin
    {
        private readonly IDictionary<string, Factory<ITextBitmapGenerator>> _generators =
            new Dictionary<string, Factory<ITextBitmapGenerator>>
            {
                { typeof(BrightnessBasedBitmapGenerator).Name, BrightnessBasedBitmapGenerator.Create },
            };

        private readonly Factory<ITextBitmapGenerator> _defaultGenerator = BrightnessBasedBitmapGenerator.Create;

        private readonly IDictionary<string, Factory<ITextBitmapRenderer>> _renderers =
            new Dictionary<string, Factory<ITextBitmapRenderer>>
            {
                { typeof(StringBitmapRenderer).Name, StringBitmapRenderer.Create },
                { typeof(FontBitmapRenderer).Name, FontBitmapRenderer.Create },
            };
        private readonly Factory<ITextBitmapRenderer> _defaultRenderer = FontBitmapRenderer.Create;

        public IEnumerable<string> AvailableGenerators => _generators.Keys;

        public Factory<ITextBitmapGenerator> LookupGenerator(string name)
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

        public IEnumerable<string> AvailableRenderers => _renderers.Keys;

        public Factory<ITextBitmapRenderer> LookupRenderer(string name)
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
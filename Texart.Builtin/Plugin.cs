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
        private readonly IDictionary<string, Factory<ITextGenerator>> _generators =
            new Dictionary<string, Factory<ITextGenerator>>
            {
                { typeof(BrightnessBasedGenerator).Name, BrightnessBasedGenerator.Create },
            };

        private readonly Factory<ITextGenerator> _defaultGenerator = BrightnessBasedGenerator.Create;

        private readonly IDictionary<string, Factory<ITextRenderer>> _renderers =
            new Dictionary<string, Factory<ITextRenderer>>
            {
                { typeof(StringRenderer).Name, StringRenderer.Create },
                { typeof(FontRenderer).Name, FontRenderer.Create },
            };
        private readonly Factory<ITextRenderer> _defaultRenderer = FontRenderer.Create;

        public IEnumerable<string> AvailableGenerators => _generators.Keys;

        public Factory<ITextGenerator> LookupGenerator(string name)
        {
            if (name == null)
            {
                return _defaultGenerator;
            }
            if (_generators.TryGetValue(name, out var factory))
            {
                return factory;
            }
            throw new ArgumentException($"No {nameof(ITextGenerator)} named '{name}' exists.");
        }

        public IEnumerable<string> AvailableRenderers => _renderers.Keys;

        public Factory<ITextRenderer> LookupRenderer(string name)
        {
            if (name == null)
            {
                return _defaultRenderer;
            }
            if (_renderers.TryGetValue(name, out var factory))
            {
                return factory;
            }
            throw new ArgumentException($"No {nameof(ITextRenderer)} named '{name}' exists.");
        }
    }
}
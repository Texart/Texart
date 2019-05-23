using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Texart.Builtin.Generators;
using Texart.Builtin.Renderers;
using Texart.Api;

namespace Texart.Builtin
{
    public sealed class Plugin : IPlugin
    {
        private readonly IDictionary<string, Factory<ITextBitmapGenerator, Lazy<JToken>>> _generators =
            new Dictionary<string, Factory<ITextBitmapGenerator, Lazy<JToken>>>
            {
                { typeof(BrightnessBasedBitmapGenerator).Name, BrightnessBasedBitmapGenerator.Create },
            };

        private readonly Factory<ITextBitmapGenerator, Lazy<JToken>> _defaultGenerator = BrightnessBasedBitmapGenerator.Create;

        private readonly IDictionary<string, Factory<ITextBitmapRenderer, Lazy<JToken>>> _renderers =
            new Dictionary<string, Factory<ITextBitmapRenderer, Lazy<JToken>>>
            {
                { typeof(StringBitmapRenderer).Name, StringBitmapRenderer.Create },
                { typeof(FontBitmapRenderer).Name, FontBitmapRenderer.Create },
            };
        private readonly Factory<ITextBitmapRenderer, Lazy<JToken>> _defaultRenderer = FontBitmapRenderer.Create;

        public IEnumerable<string> AvailableGenerators => _generators.Keys;

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

        public IEnumerable<string> AvailableRenderers => _renderers.Keys;

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
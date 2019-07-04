using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

#nullable enable

namespace Texart.Api
{
    using Locator = TxPluginResourceLocator;
    using RelativeLocator = TxPluginResourceLocator.RelativeLocator;

    public sealed partial class TxPluginBuilder : ICloneable
    {
        public TxPluginBuilder AddGenerator(
            RelativeLocator locator,
            TxPluginResource<ITxTextBitmapGenerator> generator,
            IReadOnlyList<string>? help = null)
        {
            if (locator is null)
            {
                throw new ArgumentNullException(nameof(locator));
            }
            if (generator is null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            if (_state.Generators.ContainsKey(locator))
            {
                throw new ArgumentException($"{nameof(ITxTextBitmapGenerator)} named '{locator}' already exists");
            }
            _state.Generators.Add(locator, WithHelp.Of(generator, help));

            return this;
        }

        public TxPluginBuilder AddGenerator(
            RelativeLocator locator,
            TxFactory<ITxTextBitmapGenerator, TxArguments> generator,
            IReadOnlyList<string>? help = null) => AddGenerator(locator, TxPluginResource.OfGeneratorFactory(generator), help);

        public TxPluginBuilder AddGenerator(
            Type type,
            TxPluginResource<ITxTextBitmapGenerator> generator,
            IReadOnlyList<string>? help = null)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return AddGenerator(Locator.OfRelativeResource(type.Name), generator, help);
        }

        public TxPluginBuilder AddGenerator(
            Type type,
            TxFactory<ITxTextBitmapGenerator, TxArguments> generator,
            IReadOnlyList<string>? help = null) => AddGenerator(type, TxPluginResource.OfGeneratorFactory(generator), help);

        public TxPluginBuilder AddDefaultGenerator(
            TxPluginResource<ITxTextBitmapGenerator> generator,
            IReadOnlyList<string>? help = null) => AddGenerator(Locator.OfRelativeResource(string.Empty), generator, help);

        public TxPluginBuilder AddDefaultGenerator(
            TxFactory<ITxTextBitmapGenerator, TxArguments> generator,
            IReadOnlyList<string>? help = null) => AddGenerator(Locator.OfRelativeResource(string.Empty), generator, help);

        public TxPluginBuilder AddRenderer(
            RelativeLocator locator,
            TxPluginResource<ITxTextBitmapRenderer> renderer,
            IReadOnlyList<string>? help = null)
        {
            if (locator is null)
            {
                throw new ArgumentNullException(nameof(locator));
            }
            if (renderer is null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            if (_state.Renderers.ContainsKey(locator))
            {
                throw new ArgumentException($"{nameof(ITxTextBitmapRenderer)} named '{locator}' already exists");
            }
            _state.Renderers.Add(locator, WithHelp.Of(renderer, help ?? ImmutableArray<string>.Empty));
            return this;
        }

        public TxPluginBuilder AddRenderer(
            RelativeLocator locator,
            TxFactory<ITxTextBitmapRenderer, TxArguments> renderer,
            IReadOnlyList<string>? help = null) => AddRenderer(locator, TxPluginResource.OfRendererFactory(renderer), help);

        public TxPluginBuilder AddRenderer(
            Type type,
            TxFactory<ITxTextBitmapRenderer, TxArguments> renderer,
            IReadOnlyList<string>? help = null) => AddRenderer(type, TxPluginResource.OfRendererFactory(renderer), help);

        public TxPluginBuilder AddRenderer(
            Type type,
            TxPluginResource<ITxTextBitmapRenderer> renderer,
            IReadOnlyList<string>? help = null)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return AddRenderer(Locator.OfRelativeResource(type.Name), renderer, help);
        }

        public TxPluginBuilder AddDefaultRenderer(
            TxPluginResource<ITxTextBitmapRenderer> renderer,
            IReadOnlyList<string>? help = null) => AddRenderer(Locator.OfRelativeResource(string.Empty), renderer, help);

        public TxPluginBuilder AddDefaultRenderer(
            TxFactory<ITxTextBitmapRenderer, TxArguments> renderer,
            IReadOnlyList<string>? help = null) => AddRenderer(Locator.OfRelativeResource(string.Empty), renderer, help);

        public TxPluginBuilder AddPackage(RelativeLocator locator, IReadOnlyList<string>? help = null)
        {
            if (locator is null)
            {
                throw new ArgumentNullException(nameof(locator));
            }

            if (_state.Packages.Any(p => p.Value == locator))
            {
                throw new ArgumentException($"package named '{locator}' already exists");
            }
            if (!_state.Generators.ContainsKey(locator))
            {
                throw new ArgumentException($"No {nameof(ITxTextBitmapGenerator)} named '{locator}' exists.");
            }
            if (!_state.Renderers.ContainsKey(locator))
            {
                throw new ArgumentException($"No {nameof(ITxTextBitmapRenderer)} named '{locator}' exists.");
            }
            _state.Packages.Add(WithHelp.Of(locator, help));
            return this;
        }

        public TxPluginBuilder AddPluginHelp(IReadOnlyList<string> help)
        {
            _state.Help = help ?? throw new ArgumentNullException(nameof(help));
            return this;
        }

        public ITxPlugin CreatePlugin()
            => new BuilderStatePlugin(_state);

        public TxPluginBuilder Clone() =>
            new TxPluginBuilder(new BuilderStatePlugin());

        /// <inheritdoc/>
        object ICloneable.Clone() => Clone();

        public TxPluginBuilder() : this(new BuilderStatePlugin()) { }

        private TxPluginBuilder(BuilderStatePlugin state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        /// <summary>
        /// Internal instance that represents the current (transient) plugin state for <c>this</c> builder.
        /// When <c>this</c> builder is updated, this value should also be mutated.
        /// </summary>
        private readonly BuilderStatePlugin _state;

        /// <summary>
        /// A <see cref="ITxPlugin"/> implementation that helps <see cref="TxPluginBuilder"/> by tracking the "state"
        /// of the plugin that represents the current description. When <see cref="TxPluginBuilder"/> is updated,
        /// this should also be mutated.
        /// </summary>
        private sealed class BuilderStatePlugin : ITxPlugin
        {
            public readonly Dictionary<RelativeLocator, WithHelp<TxPluginResource<ITxTextBitmapGenerator>>> Generators;
            public readonly Dictionary<RelativeLocator, WithHelp<TxPluginResource<ITxTextBitmapRenderer>>> Renderers;
            public readonly List<WithHelp<RelativeLocator>> Packages;
            public IReadOnlyList<string> Help;

            internal BuilderStatePlugin() : this(
                new Dictionary<RelativeLocator, WithHelp<TxPluginResource<ITxTextBitmapGenerator>>>(),
                new Dictionary<RelativeLocator, WithHelp<TxPluginResource<ITxTextBitmapRenderer>>>(),
                new List<WithHelp<RelativeLocator>>(),
                ImmutableArray<string>.Empty)
            { }

            internal BuilderStatePlugin(BuilderStatePlugin other) : this(
                other.Generators, other.Renderers, other.Packages, other.Help)
            { }

            private BuilderStatePlugin(
                Dictionary<RelativeLocator, WithHelp<TxPluginResource<ITxTextBitmapGenerator>>> generators,
                Dictionary<RelativeLocator, WithHelp<TxPluginResource<ITxTextBitmapRenderer>>> renderers,
                List<WithHelp<RelativeLocator>> packages,
                IReadOnlyList<string> help)
            {
                Generators = generators ?? throw new ArgumentNullException(nameof(generators));
                Renderers = renderers ?? throw new ArgumentNullException(nameof(renderers));
                Packages = packages ?? throw new ArgumentNullException(nameof(packages));
                Help = help ?? throw new ArgumentNullException(nameof(help));
            }

            /// <inheritdoc/>
            IEnumerable<RelativeLocator> ITxPlugin.AvailableGenerators => Generators.Keys;

            /// <inheritdoc/>
            TxPluginResource<ITxTextBitmapGenerator> ITxPlugin.LookupGenerator(Locator locator)
            {
                if (locator is null)
                {
                    throw new ArgumentNullException(nameof(locator));
                }
                if (Generators.TryGetValue(locator.RelativeResource, out var resource))
                {
                    return resource.Value;
                }
                throw new ArgumentException($"No {nameof(ITxTextBitmapGenerator)} named '{locator.RelativeResource}' exists.");
            }

            /// <inheritdoc/>
            IEnumerable<RelativeLocator> ITxPlugin.AvailableRenderers => Renderers.Keys;

            /// <inheritdoc/>
            TxPluginResource<ITxTextBitmapRenderer> ITxPlugin.LookupRenderer(Locator locator)
            {
                if (locator is null)
                {
                    throw new ArgumentNullException(nameof(locator));
                }
                if (Renderers.TryGetValue(locator.RelativeResource, out var resource))
                {
                    return resource.Value;
                }
                throw new ArgumentException($"No {nameof(ITxTextBitmapRenderer)} named '{locator.RelativeResource}' exists.");
            }

            /// <inheritdoc/>
            IEnumerable<RelativeLocator> ITxPlugin.AvailablePackages => Packages.Select(p => p.Value);

            /// <inheritdoc/>
            void ITxPlugin.PrintHelp(ITxConsole console)
            {
                foreach (var line in Help)
                {
                    console.Out.Write(line);
                    console.Out.Write(Environment.NewLine);
                }
            }

            /// <inheritdoc/>
            void ITxPlugin.PrintHelp(ITxConsole console, TxPluginResourceKind resourceKind, Locator locator)
            {
                IReadOnlyList<string>? help = null;
                switch (resourceKind)
                {
                    case TxPluginResourceKind.Generator:
                        {
                            if (Generators.TryGetValue(locator.RelativeResource, out var resource))
                            {
                                help = resource.Help;
                            }
                            else
                            {
                                throw new ArgumentException(
                                    $"No {nameof(ITxTextBitmapGenerator)} named '{locator.RelativeResource}' exists.");
                            }
                            break;
                        }
                    case TxPluginResourceKind.Renderer:
                        {
                            if (Renderers.TryGetValue(locator.RelativeResource, out var resource))
                            {
                                help = resource.Help;
                            }
                            else
                            {
                                throw new ArgumentException(
                                    $"No {nameof(ITxTextBitmapRenderer)} named '{locator.RelativeResource}' exists.");
                            }
                            break;
                        }
                    case TxPluginResourceKind.Package:
                        {
                            help = Packages.Find(p => p.Value == locator.RelativeResource)?.Help;
                            if (help is null)
                            {
                                throw new ArgumentException(
                                    $"No package named '{locator.RelativeResource}' exists.");
                            }
                            break;
                        }
                }
                Debug.Assert(help != null);
                // TODO: Add better help formatting
                foreach (var line in Help)
                {
                    console.Out.Write(line);
                    console.Out.Write(Environment.NewLine);
                }
            }
        }

        /// <summary>
        /// Wrapper that stores help string associated with a value of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of value to store help for.</typeparam>
        internal sealed class WithHelp<T>
        {
            /// <summary>
            /// The stored value.
            /// </summary>
            public T Value { get; }
            /// <summary>
            /// The help string associated with <see cref="Value"/>.
            /// </summary>
            public IReadOnlyList<string> Help { get; }
            /// <summary>
            /// Constructs a wrapped instance.
            /// </summary>
            /// <param name="value">The value to store.</param>
            /// <param name="help">The help string associated with <paramref name="value"/>.</param>
            internal WithHelp(T value, IReadOnlyList<string> help)
            {
                Value = value;
                Help = help;
            }
        }

        internal static class WithHelp
        {
            public static WithHelp<T> Of<T>(T value, IReadOnlyList<string>? help = null) =>
                new WithHelp<T>(value, help ?? ImmutableArray<string>.Empty);
        }
    }
}
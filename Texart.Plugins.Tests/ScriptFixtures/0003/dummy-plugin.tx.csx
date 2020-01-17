#r "SkiaSharp.dll"
#r "Texart.Api.dll"

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SkiaSharp;
using Texart.Api;

using Locator = Texart.Api.TxPluginResourceLocator;
using RelativeLocator = Texart.Api.TxPluginResourceLocator.RelativeLocator;

sealed class DummyPlugin : ITxPlugin
{
    public IEnumerable<RelativeLocator> AvailableGenerators => ImmutableArray<RelativeLocator>.Empty;

    public TxPluginResource<ITxTextBitmapGenerator> LookupGenerator(Locator name) =>
        throw new NotImplementedException();

    public IEnumerable<RelativeLocator> AvailableRenderers => ImmutableArray<RelativeLocator>.Empty;

    public TxPluginResource<ITxTextBitmapRenderer> LookupRenderer(Locator name) =>
        throw new NotImplementedException();

    public IEnumerable<RelativeLocator> AvailablePackages => ImmutableArray<RelativeLocator>.Empty;

    public (RelativeLocator generator, RelativeLocator renderer) LookupPackage(Locator locator) =>
        throw new NotImplementedException();

    public void PrintHelp(ITxConsole console) =>
        throw new NotImplementedException();

    public void PrintHelp(ITxConsole console, TxPluginResourceKind resourceKind, TxPluginResourceLocator locator) =>
        throw new NotImplementedException();
}

return new DummyPlugin();

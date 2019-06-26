using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using SkiaSharp;
using Texart.Api;

namespace Texart
{
    public class CommandLineArgs
    {
        private const string TexartDescription = "Modern ASCII-art platform";
        private static IReadOnlyList<string> HelpTokens => new []{ "--help", "-h", "-?", "/?" };

        public static CommandLineArgs Parse(string[] args)
        {
            var builder = new CommandLineBuilder(new RootCommand(TexartDescription)
            {
                TreatUnmatchedTokensAsErrors = false
            });
            // TODO: figure out why this doesn't do anything
            builder.UseHelp(HelpTokens);

            var versionOption = CreateVersionOption();
            var resizeOption = CreateResizeOption();
            builder.AddOption(versionOption);
            builder.AddOption(resizeOption);

            var parser = builder.Build();
            var parseResult = parser.Parse("-v".Split(' '));
            // var parseResult = parser.Parse("--help --resize 1000x1000 --package tx:///plugin.dll:resource".Split(' '));
            // var resize = parseResult.FindResultFor(resizeOption).GetValueOrDefault();
            return null;
        }

        private static Option CreateVersionOption() =>
            new Option(new []{ "--version", "-v" }, "Display the version of Texart");

        private static Option CreateResizeOption()
        {
            return new Option("--resize", "Resize the input image dimensions before proceeding")
            {
                Argument = new Argument<SKSizeI>(TryParseSizeI)
            };
            static bool TryParseSizeI(SymbolResult symbolResult, out SKSizeI value) =>
                TxSize.TryParseI(symbolResult.Token.Value, out value);
        }
    }
}
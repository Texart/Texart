using System.Collections.Generic;
using NUnit.Framework;

namespace Texart.Api.Tests
{
    [TestFixture]
    internal class TxArgumentsTests
    {
        private struct AllowsExtractionExtractable : TxArguments.IExtractable
        {
            public int Foo { get; private set; }
            void TxArguments.IExtractable.Extract(TxArguments args)
            {
                Foo = args.GetInt("foo");
            }
        }

        [Test]
        public void AllowsExtraction()
        {
            var args = new TxArguments(new Dictionary<string, string>{ {"foo", "42"} });
            var e = args.Extract<AllowsExtractionExtractable>();
            Assert.AreEqual(42, e.Foo);
        }
    }
}
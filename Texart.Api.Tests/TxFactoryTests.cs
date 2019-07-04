using System.Collections.Generic;
using NUnit.Framework;

namespace Texart.Api.Tests
{
    [TestFixture]
    internal class TxFactoryTests
    {
        [Test]
        public void AllowsDelegateBehavior()
        {
            // sanity check to make sure that it can be used like a delegate
            TxFactory<int, TxArguments> factory = args => args.GetInt("key");
            var arguments = new TxArguments(new Dictionary<string, string> {{ "key", "5" }});
            int factoryResult = factory(arguments);
            Assert.AreEqual(5, factoryResult);
        }
    }
}
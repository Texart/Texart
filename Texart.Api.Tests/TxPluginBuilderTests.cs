using NUnit.Framework;

namespace Texart.Api.Tests
{
    [TestFixture]
    public class TxPluginBuilderTests
    {
        public void AllowsAddGeneratorWithResourceLocatorString() { }
        public void RejectsAddGeneratorWithBadResourceLocatorString() { }
        public void AllowsAddGeneratorWithType() { }
        public void RejectsDuplicateAddGenerator() { }
        public void AllowsAddGeneratorWithRedirect() { }
        public void AllowsAddGeneratorWithFactoryResource() { }
        public void AllowsAddGeneratorWithHelp() { }

        public void AllowsSetHelp() { }
        public void AllowsMultipleSetHelp() { }

        public void AllowsAddPackage() { }
        public void RejectsAddPackageWithMissing() { }
    }
}
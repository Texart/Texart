using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Texart.Plugins.Scripting;

namespace Texart.Plugins.Tests.Scripting
{
    internal class TranslationUnitTests
    {
        [Test]
        public async Task AllowsCompilation()
        {
            const string code = @"
#load ""file:hello2.csx""
return new Hello2().DoStuff();
";
            
            var tu = new TranslationUnit(new SourceFile("C:/Code/Texart/hello.csx", code));
            var result = await tu.RunAsync();
            Assert.AreEqual(5, result);
        }
    }
}

using System.Threading.Tasks;
using NUnit.Framework;
using Texart.Plugins.Scripting;

namespace Texart.Plugins.Tests.Scripting
{
    internal class PluginScriptTests
    {
        [Test]
        public async Task AllowsCompilation()
        {
            const string code = @"
#load ""file:hello2.csx""
return new Hello2().DoStuff();
";
            
            var script = PluginScript.From(new SourceFile("C:/Code/Texart/hello.csx", code));
            var result = (await script.RunAsync()).ReturnValue;
        }
    }
}

using System;
using NUnit.Framework;

namespace Texart.Plugins.Tests
{
    internal class PluginResourceLocatorTests
    {
        [Test]
        public void AllowsAssemblyAndResource()
        {
            var locator = AssertValidLocator("file:///Texart.SomePlugin.dll//SomeResource");
            Assert.AreEqual(new ReferenceScheme("file"), locator.Scheme);
            Assert.AreEqual("Texart.SomePlugin.dll", locator.AssemblyPath);
            Assert.AreEqual("SomeResource", locator.ResourcePath);
        }

        private static PluginResourceLocator AssertValidLocator(string uri)
        {
            PluginResourceLocator instance = null;
            void CreateInstance()
            {
                instance = PluginResourceLocator.FromUri(new Uri(uri));
            }

            Assert.DoesNotThrow(CreateInstance);
            Assert.NotNull(instance, $"{nameof(PluginResourceLocator.FromUri)} returned null");
            return instance;
        }
            

        private static void AssertInvalidLocator(string uri)
        {
            var ex = Assert.Throws<ArgumentException>(() => PluginResourceLocator.FromUri(new Uri(uri)));
            Assert.IsTrue(
                ex.Message.StartsWith("URI "),
                "Exception was thrown but not because of invalid locator");
        }
    }
}
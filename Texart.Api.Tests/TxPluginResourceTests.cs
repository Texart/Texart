using System;
using NUnit.Framework;

namespace Texart.Api.Tests
{
    using MemberKind = TxPluginResource.MemberKind;
    using InactiveUnionMemberAccessException = TxPluginResource.InactiveUnionMemberAccessException;

    [TestFixture]
    internal class TxPluginResourceTests
    {
        [Test]
        public void HoldsFactory()
        {
            // ReSharper disable once ConvertToLocalFunction
            TxFactory<object, TxArguments> factory = args => throw new InvalidOperationException();
            var resource = TxPluginResource.OfFactory(factory);
            AssertActiveMember(resource, MemberKind.Factory, factory);
        }

        [Test]
        public void HoldsLocator()
        {
            var locator = TxPluginResourceLocator.Of("tx:///hello:world");
            var resource = TxPluginResource.Redirect<object>(locator);
            AssertActiveMember(resource, MemberKind.Redirect, new TxPluginResource.ResourceRedirect(locator, null));
        }

        private static void AssertActiveMember<T>(TxPluginResource<T> resource, MemberKind expectedKind, object expected)
        {
            Assert.AreEqual(expectedKind, resource.ActiveMemberKind);
            switch (expectedKind)
            {
                case MemberKind.Factory:
                    Assert.AreEqual(expected, resource.Factory);
                    Assert.Throws<InactiveUnionMemberAccessException>(() => { var _ = resource.Redirect; });
                    break;
                case MemberKind.Redirect:
                    Assert.AreEqual(expected, resource.Redirect);
                    Assert.Throws<InactiveUnionMemberAccessException>(() => { var _ = resource.Factory; });
                    break;
                default:
                    Assert.Fail($"Unknown member kind: {expectedKind}");
                    break;
            }
            Assert.AreEqual(resource.ActiveMember, expected);
        }

        [Test]
        public void HasCorrectEquality()
        {
            // ReSharper disable once ConvertToLocalFunction
            TxFactory<object, TxArguments> factory = args => throw new InvalidOperationException();
            var resource1 = TxPluginResource.OfFactory(factory);
            var resource2 = TxPluginResource.OfFactory(factory);

            // ReSharper disable once ConvertToLocalFunction
            Func<TxArguments, TxArguments> argumentsTransformer = args => args;
            var resource3 = TxPluginResource.Redirect<object>(TxPluginResourceLocator.Of("tx:///hello:world"),
                argumentsTransformer);
            var resource4 = TxPluginResource.Redirect<object>(TxPluginResourceLocator.Of("tx:///hello:world"),
                argumentsTransformer);

            Assert.IsTrue(resource1 == resource2);
            Assert.IsTrue(resource2 == resource1);
            Assert.IsFalse(resource1 != resource2);
            Assert.IsFalse(resource2 != resource1);

            Assert.IsTrue(resource3 == resource4);
            Assert.IsTrue(resource4 == resource3);
            Assert.IsFalse(resource3 != resource4);
            Assert.IsFalse(resource4 != resource3);

            Assert.IsFalse(resource1 == resource3);
            Assert.IsFalse(resource1 == resource4);
            Assert.IsFalse(resource2 == resource3);
            Assert.IsFalse(resource2 == resource4);
            Assert.IsTrue(resource1 != resource3);
            Assert.IsTrue(resource1 != resource4);
            Assert.IsTrue(resource2 != resource3);
            Assert.IsTrue(resource2 != resource4);
        }

        [Test]
        public void HasCorrectHashCodes()
        {
            // ReSharper disable once ConvertToLocalFunction
            TxFactory<object, TxArguments> factory = args => throw new InvalidOperationException();
            var resource1 = TxPluginResource.OfFactory(factory);
            var resource2 = TxPluginResource.OfFactory(factory);

            // ReSharper disable once ConvertToLocalFunction
            Func<TxArguments, TxArguments> argumentsTransformer = args => args;
            var resource3 = TxPluginResource.Redirect<object>(TxPluginResourceLocator.Of("tx:///hello:world"),
                argumentsTransformer);
            var resource4 = TxPluginResource.Redirect<object>(TxPluginResourceLocator.Of("tx:///hello:world"),
                argumentsTransformer);

            Assert.AreEqual(resource1.GetHashCode(), resource2.GetHashCode());
            Assert.AreEqual(resource3.GetHashCode(), resource4.GetHashCode());
        }

        /// <summary>
        /// Tests for <see cref="TxPluginResource.ResourceRedirect"/>.
        /// </summary>
        [TestFixture]
        internal class ResourceRedirectTests
        {
            [Test]
            public void AllowsNullArgumentsTransformer()
            {
                var redirect = new TxPluginResource.ResourceRedirect(TxPluginResourceLocator.Of("tx:///hello:world"), null);
                Assert.AreEqual(TxPluginResourceLocator.Of("tx:///hello:world"), redirect.Locator);
                Assert.AreEqual(null, redirect.ArgumentsTransformer);
            }
        }
    }
}
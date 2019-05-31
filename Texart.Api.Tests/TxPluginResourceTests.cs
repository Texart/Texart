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
            TxFactory<object, TxArguments> factory = args => throw new InvalidOperationException();
            var resource = TxPluginResource.OfFactory(factory);
            AssertActiveMember(resource, MemberKind.Factory, factory);
        }

        [Test]
        public void HoldsLocator()
        {
            var locator = TxPluginResourceLocator.Of("tx:///hello:world");
            var resource = TxPluginResource.OfLocator<object>(locator);
            AssertActiveMember(resource, MemberKind.Locator, locator);
        }

        private static void AssertActiveMember<T>(TxPluginResource<T> resource, MemberKind expectedKind, object expected)
        {
            Assert.AreEqual(expectedKind, resource.ActiveMemberKind);
            switch (expectedKind)
            {
                case MemberKind.Factory:
                    Assert.AreEqual(expected, resource.Factory);
                    Assert.Throws<InactiveUnionMemberAccessException>(() => { var _ = resource.Locator; });
                    break;
                case MemberKind.Locator:
                    Assert.AreEqual(expected, resource.Locator);
                    Assert.Throws<InactiveUnionMemberAccessException>(() => { var _ = resource.Factory; });
                    break;
                default:
                    Assert.Fail($"Unknown member kind: {expectedKind}");
                    break;
            }
            Assert.AreEqual(resource.ActiveMember, expected);
        }
    }
}
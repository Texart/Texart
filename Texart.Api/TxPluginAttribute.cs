using System;

namespace Texart.Api
{
    /// <summary>
    /// A <see cref="TxPluginAttribute"/> "exports" the <see cref="ITxPlugin"/> type from a prebuilt
    /// assembly. Texart will automatically search the assembly for this attribute and instantiate an
    /// instance.
    /// This attribute can only be applied to classes. The applied class <b>MUST</b> implement
    /// <see cref="ITxPlugin"/> and <b>MUST</b> be have a <c>public</c> constructor that takes no arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TxPluginAttribute : Attribute
    {
    }
}
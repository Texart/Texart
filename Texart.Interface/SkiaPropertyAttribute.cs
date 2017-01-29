using System;

namespace Texart.Interface
{
    /// <summary>
    /// An attribute for stating that a property is the underlying Skia type that the current
    /// class is wrapping or using.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class SkiaPropertyAttribute : Attribute
    {
    }
}

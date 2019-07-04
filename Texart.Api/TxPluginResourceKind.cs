#nullable enable

namespace Texart.Api
{
    /// <summary>
    /// A tag to represent types of resources that <see cref="ITxPlugin"/> provides.
    /// </summary>
    public enum TxPluginResourceKind
    {
        /// <summary>
        /// A tag representing an <see cref="ITxTextBitmapGenerator"/> resource.
        /// </summary>
        Generator,
        /// <summary>
        /// A tag representing an <see cref="ITxTextBitmapRenderer"/> resource.
        /// </summary>
        Renderer,
        /// <summary>
        /// A tag representing a package resource.
        /// </summary>
        Package
    }
}
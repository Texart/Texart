namespace Texart.ScriptInterface
{
    public struct Font
    {
        /// <summary>
        /// The desired number of pixels that represents the length
        /// of the bounding box for one character.
        /// </summary>
        public int DesiredCharacterSpacing { get; set; }
        /// <summary>
        /// The desired font size.
        /// </summary>
        public float TextSize { get; set; }
        /// <summary>
        /// The underlying typeface.
        /// </summary>
        public Typeface Typeface { get; set; }
    }
}

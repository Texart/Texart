using SkiaSharp;

namespace Texart.ScriptInterface
{
    public struct Color
    {
        internal SKColor SkiaColor { get; set; }

        public Color(byte r, byte g, byte b, byte a = 255)
        {
            this.SkiaColor = new SKColor(r, g, b, a);
        }

        internal Color(SKColor color)
        {
            this.SkiaColor = color;
        }

        byte R
        {
            get { return this.SkiaColor.Red; }
            set { this.SkiaColor = this.SkiaColor.WithRed(value); }
        }

        byte G
        {
            get { return this.SkiaColor.Green; }
            set { this.SkiaColor = this.SkiaColor.WithGreen(value); }
        }

        byte B
        {
            get { return this.SkiaColor.Blue; }
            set { this.SkiaColor = this.SkiaColor.WithBlue(value); }
        }

        byte A
        {
            get { return this.SkiaColor.Alpha; }
            set { this.SkiaColor = this.SkiaColor.WithAlpha(value); }
        }

        public Color WithRed(byte value)
        {
            return new Color() { SkiaColor = this.SkiaColor.WithRed(value) };
        }

        public Color WithGreen(byte value)
        {
            return new Color() { SkiaColor = this.SkiaColor.WithGreen(value) };
        }

        public Color WithBlue(byte value)
        {
            return new Color() { SkiaColor = this.SkiaColor.WithBlue(value) };
        }

        public Color WithAlpha(byte value)
        {
            return new Color() { SkiaColor = this.SkiaColor.WithAlpha(value) };
        }
    }
}

﻿using SkiaSharp;
using System.IO;

namespace Texart.Interface
{
    public class Bitmap
    {
        [SkiaProperty]
        public SKBitmap SkiaBitmap { get; set; }

        public int Width => this.SkiaBitmap.Width;
        public int Height => this.SkiaBitmap.Height;

        public static Bitmap FromFile(string fileName)
        {
            using (Stream fileStream = File.OpenRead(fileName))
            using (SKStream managedStream = new SKManagedStream(fileStream))
            {
                return new Bitmap() { SkiaBitmap = SKBitmap.Decode(managedStream) };
            }
        }

        ~Bitmap()
        {
            this.SkiaBitmap.Dispose();
        }

        /// <summary>
        /// Private constructor. Does nothing.
        /// </summary>
        private Bitmap() { }
    }
}

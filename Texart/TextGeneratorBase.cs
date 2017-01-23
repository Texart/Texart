using System.Diagnostics;
using System.Collections.Generic;
using SkiaSharp;
using System;

namespace Texart
{
    public abstract class TextGeneratorBase : ITextGenerator
    {
        /// <inheritdocs/>
        public abstract IList<char> Characters { get; }

        /// <inheritdocs/>
        public SKBitmap Bitmap { get; protected set; }

        /// <inheritdocs/>
        public int PixelSamplingRatio { get; protected set; }

        /// <inheritdocs/>
        public int Width { get { return Bitmap.Width / PixelSamplingRatio; } }

        /// <inheritdocs/>
        public int Height { get { return Bitmap.Height / PixelSamplingRatio; } }

        /// <inheritdocs/>
        public abstract ITextData GenerateText();

        /// <summary>
        /// Creates a <code>LockedBitmapAccessor</code> from <code>bitmap</code>.
        /// </summary>
        /// <param name="bitmap">The bitmap whose pixels to lock</param>
        /// <returns>An object which provides access to <code>bitmap</code> with locked pixels</returns>
        /// <see cref="LockedBitmapAccessor"/>
        protected static LockedBitmapAccessor LockedBitmap(SKBitmap bitmap)
        {
            if (bitmap == null) { throw new ArgumentNullException(nameof(bitmap)); }
            return new LockedBitmapAccessor(bitmap);
        }

        /// <summary>
        /// A class that locks an <code>SkBitmap</code>'s pixels when constructed
        /// and unlocks pixels when disposed.
        /// </summary>
        /// <see cref="SKBitmap.LockPixels"/>
        /// <see cref="SKBitmap.UnlockPixels"/>
        protected class LockedBitmapAccessor : IDisposable
        {
            /// <summary>
            /// The locked bitmap
            /// </summary>
            public SKBitmap Bitmap { get; }

            /// <summary>
            /// Locks the provided bitmap's pixels and stores it to unlock them later.
            /// </summary>
            /// <param name="bitmap">The bitmap whose pixels to lock</param>
            internal LockedBitmapAccessor(SKBitmap bitmap)
            {
                Debug.Assert(bitmap != null);
                Bitmap = bitmap;

                Bitmap.LockPixels();
            }
            
            /// <inheritdocs/>
            public void Dispose()
            {
                Bitmap.UnlockPixels();
            }
        }
    }
}

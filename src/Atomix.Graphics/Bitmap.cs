using System;
using System.Collections.Generic;

namespace Atomix.Graphics
{

    /// <summary>
    /// Bitmap.
    /// </summary>
    public unsafe class Bitmap
    {

        /// <summary>
        /// The pixels.
        /// </summary>
        readonly int* pixels;

        /// <summary>
        /// The width.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// The height.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// Gets the size of the bitmap in bytes.
        /// </summary>
        /// <value>The size.</value>
        public int Size => Width * Height * 4;

        public Bitmap(int width, int height)
        {
            Width = width;
            Height = height;
            pixels = (int *) Allocator.Alloc(width * height * 4);
        }

        public void SetPixel(int x, int y, byte r, byte g, byte b, byte a)
        {
            ClampCoords(&x, &y);
            int rgba = a << 32 | b << 16 | g << 8 | r;
            *(pixels + (y * Width + x)) = rgba;
        }

        void ClampCoords(int *x, int *y)
        {
            ClampPtr(x, 0, Width);
            ClampPtr(y, 0, Height);
        }

        void ClampPtr(int *x, int min, int max)
        {
            *x = *x < min ? min : *x > max ? max : *x;
        }
    }
}


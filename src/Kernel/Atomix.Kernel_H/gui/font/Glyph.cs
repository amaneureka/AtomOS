using System;

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.gui.font
{
    public unsafe class Glyph
    {
        /// <summary>
        /// Real Width of font glyph bitamp
        /// </summary>
        public uint Width;
        /// <summary>
        /// Real Height of font glyph bitamp
        /// </summary>
        public uint Height;
        /// <summary>
        /// Left offset
        /// </summary>
        public uint xOffset;
        /// <summary>
        /// Upward offset
        /// </summary>
        public uint yOffset;
        /// <summary>
        /// Decimal value corresponding to this font glyph
        /// </summary>
        public uint Unicode;
        /// <summary>
        /// Aparent width of font glyph bitamp
        /// </summary>
        public uint DWidth;
        /// <summary>
        /// Glyph data
        /// </summary>
        public uint* Bitmap;
    }
}

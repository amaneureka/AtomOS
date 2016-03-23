using System;

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.gui.font
{
    [StructLayout(LayoutKind.Explicit, Size = 28)]
    public unsafe struct Glyph
    {
        /// <summary>
        /// Real Width of font glyph bitamp
        /// </summary>
        [FieldOffset(0)]
        public uint Width;
        /// <summary>
        /// Real Height of font glyph bitamp
        /// </summary>
        [FieldOffset(4)]
        public uint Height;
        /// <summary>
        /// Left offset
        /// </summary>
        [FieldOffset(8)]
        public uint xOffset;
        /// <summary>
        /// Upward offset
        /// </summary>
        [FieldOffset(12)]
        public uint yOffset;
        /// <summary>
        /// Aparent width of font glyph bitamp
        /// </summary>
        [FieldOffset(16)]
        public uint DWidth;
        /// <summary>
        /// Aparent width of font glyph bitamp
        /// </summary>
        [FieldOffset(20)]
        public uint* Bitmap;
        /// <summary>
        /// Decimal value corresponding to this font glyph
        /// </summary>
        [FieldOffset(24)]
        public uint Unicode;
    }
}

using System;

namespace Atomix.Kernel_H.gui.font.BDF
{
    public class Glyph
    {
        /// <summary>
        /// Real Width of font glyph bitamp
        /// </summary>
        public int Width;
        /// <summary>
        /// Real Height of font glyph bitamp
        /// </summary>
        public int Height;
        /// <summary>
        /// Left offset
        /// </summary>
        public int xOffset;
        /// <summary>
        /// Upward offset
        /// </summary>
        public int yOffset;
        /// <summary>
        /// Decimal value corresponding to this font glyph
        /// </summary>
        public int Character;
        /// <summary>
        /// Aparent width of font glyph bitamp
        /// </summary>
        public int DWidth;
    }
}

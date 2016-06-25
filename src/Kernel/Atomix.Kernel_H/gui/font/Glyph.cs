/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Font Glyph class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.gui.font
{
    internal unsafe class Glyph
    {
        /// <summary>
        /// Real Width of font glyph bitamp
        /// </summary>
        internal uint Width;
        /// <summary>
        /// Real Height of font glyph bitamp
        /// </summary>
        internal uint Height;
        /// <summary>
        /// Left offset
        /// </summary>
        internal uint xOffset;
        /// <summary>
        /// Upward offset
        /// </summary>
        internal uint yOffset;
        /// <summary>
        /// Decimal value corresponding to this font glyph
        /// </summary>
        internal uint Unicode;
        /// <summary>
        /// Aparent width of font glyph bitamp
        /// </summary>
        internal uint DWidth;
        /// <summary>
        /// Glyph data
        /// </summary>
        internal uint* Bitmap;
    }
}

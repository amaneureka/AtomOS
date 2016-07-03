/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Graphics Helper class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.Lib.Graphic
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal struct Rectangle
    {
        [FieldOffset(0)]
        public int x;
        [FieldOffset(4)]
        public int y;
        [FieldOffset(8)]
        public int width;
        [FieldOffset(12)]
        public int height;
    };
}

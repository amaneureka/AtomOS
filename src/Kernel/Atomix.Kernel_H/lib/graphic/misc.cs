using System;

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.lib.graphic
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct Rectangle
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

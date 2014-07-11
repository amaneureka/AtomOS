using System;
using System.Runtime.InteropServices;

namespace libAtomixH.Threading
{
    [StructLayout (LayoutKind.Explicit, Size = 16)]
    public unsafe struct Task
    {
        [FieldOffset (0)]
        public int Pid;
        [FieldOffset (4)]
        public uint Stack;
        [FieldOffset (8)]
        public uint* Address;
        [FieldOffset (12)]
        public int state;
    };
}

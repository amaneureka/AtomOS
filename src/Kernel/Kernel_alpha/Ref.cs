using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Kernel_alpha
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public unsafe struct Task
    {
        [FieldOffset(0)]
        public int Pid;
        [FieldOffset(4)]
        public uint Stack;        
        [FieldOffset(8)]
        public uint* Address;
        [FieldOffset(12)]
        public uint state;
    };

    public enum State : uint
    {
        None = 0,
        Sleep = 1,
        Dead = 3,
        Alive = 4
    };
}

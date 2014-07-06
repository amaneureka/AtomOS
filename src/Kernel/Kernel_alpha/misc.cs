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
        public int state;
    };

    public enum State : int
    {
        None = -2,
        Dead = -1,
        Alive = 0
    };
}

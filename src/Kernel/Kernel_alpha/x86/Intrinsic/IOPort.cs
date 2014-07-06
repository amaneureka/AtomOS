using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.x86.Intrinsic
{
    // Awesome IOPorts :)
    public class IOPort
    {
        public ushort Address;

        public IOPort (ushort addr)
        {
            Address = addr;
        }

        public byte Byte
        {
            get
            {
                return Native.In8 (Address);
            }
            set
            {
                Native.Out8 (Address, value);
            }
        }

        public ushort Word
        {
            get
            {
                return Native.In16 (Address);
            }
            set
            {
                Native.Out16 (Address, value);
            }
        }

        public uint DWord
        {
            get
            {
                return Native.In32 (Address);
            }
            set
            {
                Native.Out32 (Address, value);
            }
        }
    }
}

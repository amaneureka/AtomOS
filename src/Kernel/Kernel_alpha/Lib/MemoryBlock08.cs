using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.Lib
{
    public unsafe class MemoryBlock08 : MemoryBlock
    {
        public MemoryBlock08(UInt32 Address)
        {
            this.xAddress = Address;
            this.xLength = 0;
        }

        public MemoryBlock08(UInt32 Address, UInt32 Length)
        {
            this.xAddress = Address;
            this.xLength = Length;
        }

        public byte this[uint aIndex]
        {
            get
            {
                if (aIndex >= xLength && xLength != 0)
                    throw new Exception("Memory bound exception");

                return *(byte*)(this.xAddress + aIndex);
            }
            set
            {
                if (aIndex >= xLength && xLength != 0)
                    throw new Exception("Memory overflow exception");

                *(byte*)(this.xAddress + aIndex) = value;
            }
        }
    }
}

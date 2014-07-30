using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.Lib
{
    public unsafe class MemoryBlock32 : MemoryBlock
    {
        public MemoryBlock32(UInt32 Address)
        {
            this.xAddress = Address;
            this.xLength = 0;
        }

        public MemoryBlock32(UInt32 Address, UInt32 Length)
        {
            this.xAddress = Address;
            this.xLength = Length;
        }

        public UInt32 this[uint aIndex]
        {
            get
            {
                if (aIndex >= xLength && xLength != 0)
                    throw new Exception("Memory bound exception");

                return *(UInt32*)(this.xAddress + aIndex);
            }
            set
            {
                if (aIndex >= xLength && xLength != 0)
                    throw new Exception("Memory overflow exception");

                *(UInt32*)(this.xAddress + aIndex) = value;
            }
        }
    }
}

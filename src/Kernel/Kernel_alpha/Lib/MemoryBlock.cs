using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.Lib
{
    public unsafe class MemoryBlock
    {
        private UInt32 xAddress;
        private UInt32 xLength;

        public UInt32 Address
        { 
            get { return xAddress; }
            set { xAddress = value; }
        }

        public UInt32 Length
        { 
            get { return xLength; }
            set { xLength = value; }
        }

        public MemoryBlock(UInt32 Address)
        {
            this.xAddress = Address;
            this.xLength = 0;
        }

        public MemoryBlock(UInt32 Address, UInt32 Length)
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

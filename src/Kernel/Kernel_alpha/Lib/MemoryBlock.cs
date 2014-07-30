using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.Lib
{
    public abstract class MemoryBlock
    {
        protected UInt32 xAddress;
        protected UInt32 xLength;

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
    }
}

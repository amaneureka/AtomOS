using System;

namespace libAtomixH.Core
{
    public static class Memory
    {
        public static unsafe void Clear (uint Address, uint ByteCount)
        {
            uint* xAddress = (uint*)Address;
            for (uint i = 0; i < ByteCount; i++)
            {
                xAddress[i] = 0x0;
            }
        }
    }
}

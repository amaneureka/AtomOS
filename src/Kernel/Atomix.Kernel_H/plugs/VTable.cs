using System;
using Atomix.Kernel_H.core;
using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    public static unsafe class VTableImpl
    {
        [Label("VTableImpl")]
        public static uint AddEntry(uint* aTable, uint aTypeID, uint aMethodID)
        {
            uint TypeID, MethodID, Size;
            while((Size = *aTable) != 0)
            {
                TypeID = aTable[1];
                if (TypeID == aTypeID)
                {
                    aTable += 2;
                    while((MethodID = *aTable) != 0)
                    {
                        if (MethodID == aMethodID)
                            return aTable[1];
                    }
                    throw new Exception("[VTable] Method Not Found!");
                }
                aTable += Size;
            }
            throw new Exception("[VTable] Type Not Found!");
        }
    }
}

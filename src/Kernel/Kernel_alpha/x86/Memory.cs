using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Kernel_alpha.x86.Intrinsic;
using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Kernel_alpha.x86
{
    public static class Memory
    {        
        public static unsafe void Clear(uint Address, uint ByteCount)
        {
            uint* xAddress = (uint*)Address;
            for (uint i = 0; i < ByteCount; i++)
            {
                xAddress[i] = 0x0;
            }
        }
    }
}

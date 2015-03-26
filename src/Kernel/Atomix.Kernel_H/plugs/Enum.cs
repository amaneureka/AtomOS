using System;
using System.Collections.Generic;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.Kernel_H.plugs
{
    public static unsafe class Enum
    {
        [Plug("System_Void__System_Enum__cctor__")]
        public static void Cctor(byte* Address)
        {
            return;
        }
    }
}

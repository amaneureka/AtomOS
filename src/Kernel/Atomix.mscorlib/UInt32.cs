using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.mscorlib
{
    public static class UInt32
    {
        [Plug("System_String_System_UInt32_ToString__")]
        public static string ToString(ref uint aThis)
        {
            return Number.ToString32Bit(aThis, false);
        }
    }
}

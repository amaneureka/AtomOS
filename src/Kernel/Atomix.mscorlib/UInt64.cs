using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.mscorlib
{
    public static class UInt64
    {
        [Plug("System_String_System_UInt64_ToString__")]
        public static string ToString(ref ulong aThis)
        {
            ulong x = aThis;
            return Number.ToString64Bit(x, false);
        }
    }
}

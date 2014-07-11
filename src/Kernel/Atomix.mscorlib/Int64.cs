using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.mscorlib
{
    public static class Int64
    {
        [Plug("System_String_System_Int64_ToString__")]
        public static string ToString(ref long aThis)
        {
            long x = aThis;
            if (x >= 0)
                return Number.ToString64Bit((ulong)x, false);
            else
                return Number.ToString64Bit((ulong)(x * -1), true);
        }
    }
}

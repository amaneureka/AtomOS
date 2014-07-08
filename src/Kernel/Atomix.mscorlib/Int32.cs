using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.mscorlib
{
    public static class Int32
    {
        [Plug("System_String_System_Int32_ToString__")]
        public static string ToString(ref int aThis)
        {
            int x = aThis;
            if (x >= 0)
                return Number.ToString32Bit((uint)x, false);
            else
                return Number.ToString32Bit((uint)(x * -1), true);
        }
    }
}

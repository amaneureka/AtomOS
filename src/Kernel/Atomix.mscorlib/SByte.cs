using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.mscorlib
{
    public static class SByte
    {
        [Plug("System_String_System_SByte_ToString__")]
        public static string ToString(ref sbyte aThis)
        {
            if (aThis > 0)
                return Number.ToString8Bit((byte)aThis, false);
            else
                return Number.ToString8Bit((byte)(-1 * aThis), true);
        }
    }
}

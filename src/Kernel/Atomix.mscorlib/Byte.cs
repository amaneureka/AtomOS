using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.mscorlib
{
    public static class Byte
    {
        [Plug("System_String_System_Byte_ToString__")]
        public static string ToString(ref byte aThis)
        {
            return Number.ToString8Bit(aThis, false);
        }
    }
}

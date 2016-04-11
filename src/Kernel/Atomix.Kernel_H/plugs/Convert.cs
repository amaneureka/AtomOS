using System;

using Atomix.Kernel_H.core;

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    public class Convert
    {
        [Plug("System_String_System_Convert_ToString_System_UInt32_")]
        public static string ToString(uint aThis)
        {
            return Numerics.ToString(ref aThis);
        }

        [Plug("System_String_System_Convert_ToString_System_Int32_")]
        public static string ToString(int aThis)
        {
            return Numerics.ToString(ref aThis);
        }
    }
}

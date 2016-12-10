/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          System.Covert Plugs
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomix.Kernel_H.plugs
{
    internal class Convert
    {
        [Plug("System_String_System_Convert_ToString_System_UInt32_")]
        internal static string ToString(uint aThis)
        {
            return Numerics.ToString(ref aThis);
        }

        [Plug("System_String_System_Convert_ToString_System_Int32_")]
        internal static string ToString(int aThis)
        {
            return Numerics.ToString(ref aThis);
        }
    }
}

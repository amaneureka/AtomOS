using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using System.Runtime.InteropServices;

namespace Atomix.mscorlib
{
    public static class VTable
    {
        private static uint[] xAddress = new uint[100 * 50];

        [Plug("__VTable_Get_Method__")]
        public static uint GetMethod(int aType, int aMethodIndex)
        {
            return xAddress[(int)((int)(aType * 50) + aMethodIndex)];
        }

        [Plug("__VTable_Set_Method__")]
        public static void SetMethod(int aType, int aMethodIndex, uint Address)
        {
            xAddress[(int)((int)(aType * 50) + aMethodIndex)] = Address;
        }
    }
}

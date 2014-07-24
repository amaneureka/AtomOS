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
        private static VTypes[] xTypes = new VTypes[100];

        [Plug("__VTable_Init_Methods_Table__")]
        public static void Init()
        {
            for (int i = 0; i < xTypes.Length; i++)
                xTypes[i] = new VTypes();
        }

        [Plug("__VTable_Get_Method__")]
        public static uint GetMethod(int aType, int aMethodIndex)
        {
            return xTypes[aType].xMethodAddress[aMethodIndex];
        }

        [Plug("__VTable_Set_Method__")]
        public static void SetMethod(int aType, int aMethodIndex, uint Address)
        {
            xTypes[aType].xMethodAddress[aMethodIndex] = Address;
        }
    }
    public class VTypes
    {
        public uint[] xMethodAddress = new uint[20];
    }
}

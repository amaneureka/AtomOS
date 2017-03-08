/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          helper class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Collections.Generic;

using System.Runtime.InteropServices;

namespace Atomix.Assembler
{
    public static class AssemblyHelper
    {
        public static VirtualStack vStack;
        public static List<string> NasmHeaders;
        public static List<AsmData> DataMember;
        public static List<Instruction> AssemblerCode;
        public static Dictionary<string, _MemberInfo> StaticLabels;

        public static int DataMemberBssSegmentIndex
        {
            get;
            private set;
        }

        public static void InsertData(AsmData aAsmData)
        {
            if (DataMember == null)
                throw new Exception("DataMember not initalized");

            bool IsBssData = aAsmData.IsBssData;
            if (IsBssData)
                DataMember.Insert(DataMemberBssSegmentIndex++, aAsmData);
            else
                DataMember.Add(aAsmData);
        }
    }
}

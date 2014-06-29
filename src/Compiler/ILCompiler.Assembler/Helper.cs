using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.Assembler;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Atomix.Assembler
{
    public static class AssemblyHelper
    {
        public static VirtualStack vStack;
        public static List<AsmData> DataMember;
        public static List<Instruction> AssemblerCode;
        public static Dictionary<string, _MemberInfo> StaticLabels;
    }
}

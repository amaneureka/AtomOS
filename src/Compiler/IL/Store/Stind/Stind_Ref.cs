using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.IL;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using System.Reflection;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Stind_Ref)]
    public class Stind_Ref : MSIL
    {
        public Stind_Ref(Compiler Cmp)
            : base("stindref", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            if (ILCompiler.CPUArchitecture == CompilerExt.CPUArch.x86)
                Stind_I.Stind_All(4);
            else if (ILCompiler.CPUArchitecture == CompilerExt.CPUArch.x64)
                Stind_I.Stind_All(8);
        }
    }
}

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
    [ILOp(ILCode.Conv_I)]
    public class Conv_I : MSIL
    {
        public Conv_I(Compiler Cmp)
            : base("convi", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //Conversion of pointer
            switch (ILCompiler.CPUArchitecture)
            {
                case CPUArch.x86:
                    {
                        Compiler.MSIL[ILCode.Conv_I4].Execute(instr, aMethod);
                    }
                    break;
                case CPUArch.x64:
                    {
                        Compiler.MSIL[ILCode.Conv_I8].Execute(instr, aMethod);
                    }
                    break;
            }
        }
    }
}

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
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Conv_U)]
    public class Conv_U : MSIL
    {
        public Conv_U(Compiler Cmp)
            : base("convu", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //Conversion of unsigned pointer
            switch (ILCompiler.CPUArchitecture)
            {
                case CPUArch.x86:
                    {
                        var IL = new Conv_U4(this.Compiler);
                        IL.Execute(instr, aMethod);
                    }
                    break;
                case CPUArch.x64:
                    {
                        var IL = new Conv_U8(this.Compiler);
                        IL.Execute(instr, aMethod);
                    }
                    break;
            }
        }
    }
}

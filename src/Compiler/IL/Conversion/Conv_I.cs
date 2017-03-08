/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Conv_I MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.CompilerExt;

namespace Atomix.IL
{
    [ILOp(ILCode.Conv_I)]
    [ILOp(ILCode.Conv_Ovf_I)]
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

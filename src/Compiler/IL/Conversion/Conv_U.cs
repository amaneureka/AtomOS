/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Conv_U MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.CompilerExt;

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

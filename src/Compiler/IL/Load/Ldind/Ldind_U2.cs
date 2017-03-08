/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Lding_U2 MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Ldind_U2)]
    public class Ldind_U2 : MSIL
    {
        public Ldind_U2(Compiler Cmp)
            : base("ldind_u2", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            /*
            Situation is an address of a 16 bit value is on the stack, we have to push that onto the stack
            */
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EBX });
                        Core.AssemblerCode.Add(
                            new Movzx
                            {
                                DestinationReg = Registers.EAX,
                                Size = 16,
                                SourceReg = Registers.EBX,
                                SourceIndirect = true
                            });
                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                    }
                    break;
                #endregion
                #region _x64_
                case CPUArch.x64:
                    {

                    }
                    break;
                #endregion
                #region _ARM_
                case CPUArch.ARM:
                    {

                    }
                    break;
                #endregion
            }
        }
    }
}

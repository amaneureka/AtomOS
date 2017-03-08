/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Neg MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomix.Assembler;
using Atomix.CompilerExt;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Neg)]
    public class ILNeg : MSIL
    {
        public ILNeg(Compiler Cmp)
            : base("neg", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xItem = Core.vStack.Peek();

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        if (xItem.Size > 4)
                        {
                            throw new Exception("@Neg: xItem size > 4 not implemented!");
                        }
                        else
                        {
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.EAX, SourceRef = "0x1" });
                            Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EAX, SourceRef = "0xFFFFFFFF" });
                            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                        }
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

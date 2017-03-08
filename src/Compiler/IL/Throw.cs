/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stind_Ref MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Throw)]
    public class Throw : MSIL
    {
        public Throw(Compiler Cmp)
            : base("throw", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xEndException = aMethod.FullName() + ".Error";
            if (instr.Ehc != null && instr.Ehc.HandlerOffset > instr.Position)
            {
                xEndException = ILHelper.GetLabel(aMethod, instr.Ehc.HandlerOffset);
            }
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Call(Helper.lblSetException, true));
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "0x2" });
                        Core.AssemblerCode.Add(new Jmp { DestinationRef = xEndException });
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
            Core.vStack.Pop();
        }
    }
}

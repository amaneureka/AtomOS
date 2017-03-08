/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Isinst MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Isinst)]
    public class Isinst : MSIL
    {
        public Isinst(Compiler Cmp)
            : base("isinst", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xType = ((OpType)instr).Value;
            var TypeID  = ILHelper.GetTypeID(xType);
            var xReturnNull = ILHelper.GetLabel(aMethod, instr.Position) + "_Isinst_False";
            var xReturnTrue = ILHelper.GetLabel(aMethod, instr.Position) + "_Isinst_Final";

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                        Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EAX, SourceRef = "0x" + TypeID.ToString("X"), DestinationIndirect = true });
                        Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNE, DestinationRef = xReturnNull });

                        Core.AssemblerCode.Add(new Push { DestinationRef = "0x1" });
                        Core.AssemblerCode.Add(new Jmp { DestinationRef = xReturnTrue });

                        Core.AssemblerCode.Add(new Label(xReturnNull));
                        Core.AssemblerCode.Add(new Push { DestinationRef = "0x0" });

                        Core.AssemblerCode.Add(new Label(xReturnTrue));
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

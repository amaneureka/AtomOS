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
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                        Core.AssemblerCode.Add(new Mov { DestinationRef = ((FieldInfo)Core.StaticLabels["Exception"]).FullName(), DestinationIndirect = true, SourceReg = Registers.EAX });
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

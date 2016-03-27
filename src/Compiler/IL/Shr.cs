using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.IL;
using Atomix.Assembler;
using Atomix.CompilerExt;
using System.Reflection;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Shr)]
    public class ILShr : MSIL
    {
        public ILShr(Compiler Cmp)
            : base("shr", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xStackItem_ShiftAmount = Core.vStack.Pop();
            var xStackItem_Value = Core.vStack.Peek();
            
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        if (xStackItem_Value.Size > 4)
                        {
                            throw new Exception("Not Yet implemented");
                        }
                        else
                        {
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.ECX }); //Shift Amount
                            Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceReg = Registers.CL });
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

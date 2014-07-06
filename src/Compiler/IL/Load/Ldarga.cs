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
    [ILOp(ILCode.Ldarga)]
    public class Ldarga : MSIL
    {
        public Ldarga(Compiler Cmp)
            : base("ldarga", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var aParam = ((OpVar)instr).Value;
            var xDisplacement = Ldarg.GetArgumentDisplacement(aMethod, aParam);
            
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceRef = "0x" + xDisplacement.ToString("X") });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP });
                        Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBX });
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

            Core.vStack.Push(4, typeof(uint));
        }
    }
}

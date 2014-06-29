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
    [ILOp(ILCode.Rem)]
    public class Rem : MSIL
    {
        public Rem(Compiler Cmp)
            : base("rem", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var FirstItem = Core.vStack.Pop();
            var SecondItem = Core.vStack.Pop();

            var xSize = Math.Max(FirstItem.Size, SecondItem.Size);

            /*
             * We have divisor and dividend in the stack so first pop give us divisor
             * Task is to divide them and push the remainder to the stack
             */

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        if (xSize > 4)
                        {
                            if (FirstItem.IsFloat)
                                throw new Exception("You Can't get remainder of floating point division");

                            throw new Exception("Yet to be implemented");
                        }
                        else
                        {
                            if (FirstItem.IsFloat)
                                throw new Exception("You Can't get remainder of floating point division");
                                                        
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EBX });
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Conversion { Code = ConversionCode.SignedDWord_2_SignedQWord });
                            Core.AssemblerCode.Add(new IDiv { DestinationReg = Registers.EBX });
                            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EDX });
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
            Core.vStack.Push(FirstItem.Size, FirstItem.Type);
        }
    }
}

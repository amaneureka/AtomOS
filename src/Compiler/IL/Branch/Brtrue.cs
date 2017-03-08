/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Brtrue MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using System.Reflection;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Brtrue)]
    public class Brtrue : MSIL
    {
        public Brtrue(Compiler Cmp)
            : base("brtrue", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //This is branch type IL
            var xOffset = ((OpBranch)instr).Value;
            //The brach label
            var xTrueLabel = ILHelper.GetLabel(aMethod, xOffset);
            //Just make a pop because we want only size of it
            var xSize = Core.vStack.Pop().Size;

            /*
                value is pushed onto the stack by a previous operation.
                value is popped from the stack;
                if value is true, branch to target.
            */

            //Branch to target
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        switch (xSize)
                        {
                            case 1:
                            case 2:
                            case 4:
                                {
                                    //***What we are going to do is***
                                    //1) Pop the content into EAX
                                    //2) Compare the content with 0x0 --> False
                                    //3) If they are equal than jump to branch
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EAX, SourceRef = "0x0" });
                                    Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNE, DestinationRef = xTrueLabel });
                                }
                                break;
                            case 8:
                                {
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EAX, SourceRef = "0x0" });

                                    Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNE, DestinationRef = xTrueLabel });
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EAX, SourceRef = "0x0" });
                                    Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNE, DestinationRef = xTrueLabel });
                                }
                                break;
                            default:
                                //Size > 4 is never called don't know why
                                throw new Exception("@Brtrue: Unexpected size called := " + xSize);
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

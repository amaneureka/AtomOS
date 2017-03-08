/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Rem_Un MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Rem_Un)]
    public class Rem_Un : MSIL
    {
        public Rem_Un(Compiler Cmp)
            : base("rem_un", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var FirstItem = Core.vStack.Peek();
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

                            string BaseLabel = ILHelper.GetLabel(aMethod, instr.Position) + ".";
                            string LabelShiftRight = BaseLabel + "ShiftRightLoop";
                            string LabelNoLoop = BaseLabel + "NoLoop";
                            string LabelEnd = BaseLabel + "End";

                            // divisor
                            //low
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESI, SourceReg = Registers.ESP, SourceIndirect = true });
                            //high
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = 4 });

                            //dividend
                            // low
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = 8 });
                            //high
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = 12 });

                            // pop both 8 byte values
                            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x10" });

                            // set flags
                            Core.AssemblerCode.Add(new Or { DestinationReg = Registers.EDI, SourceReg = Registers.EDI });
                            // if high dword of divisor is already zero, we dont need the loop
                            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JZ, DestinationRef = LabelNoLoop });

                            // set ecx to zero for counting the shift operations
                            Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.ECX, SourceReg = Registers.ECX });

                            Core.AssemblerCode.Add(new Label(LabelShiftRight));

                            // shift divisor 1 bit right
                            Core.AssemblerCode.Add(new Literal("shrd ESI, EDI, 0x1"));
                            Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.EDI, SourceRef = "0x1" });

                            // increment shift counter
                            Core.AssemblerCode.Add(new Literal("inc ECX"));

                            // set flags
                            Core.AssemblerCode.Add(new Or { DestinationReg = Registers.EDI, SourceReg = Registers.EDI });
                            // loop while high dword of divisor till it is zero
                            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JZ, DestinationRef = LabelShiftRight });

                            // shift the divident now in one step
                            // shift divident CL bits right
                            Core.AssemblerCode.Add(new Literal("shrd EAX, EDX, CL"));
                            Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.EDX, SourceReg = Registers.CL });

                            // so we shifted both, so we have near the same relation as original values
                            // divide this
                            Core.AssemblerCode.Add(new Div { DestinationReg = Registers.ESI });

                            // save remainder to stack
                            Core.AssemblerCode.Add(new Push { DestinationRef = "0x0" });
                            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EDX });

                            Core.AssemblerCode.Add(new Jmp { DestinationRef = LabelEnd });

                            Core.AssemblerCode.Add(new Label(LabelNoLoop));

                            //save high dividend
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EDX });
                            // zero EDX, so that high part is zero -> reduce overflow case
                            Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EDX, SourceReg = Registers.EDX });
                            // divide high part
                            Core.AssemblerCode.Add(new Div { DestinationReg = Registers.ESI });
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ECX });
                            // divide low part
                            Core.AssemblerCode.Add(new Div { DestinationReg = Registers.ESI });
                            // save remainder result
                            Core.AssemblerCode.Add(new Push { DestinationRef = "0x0" });
                            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EDX });

                            Core.AssemblerCode.Add(new Label(LabelEnd));
                        }
                        else
                        {
                            if (FirstItem.IsFloat)
                                throw new Exception("You Can't get remainder of floating point division");

                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.ECX });
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EDX, SourceReg = Registers.EDX });
                            Core.AssemblerCode.Add(new Div { DestinationReg = Registers.ECX });
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
        }
    }
}

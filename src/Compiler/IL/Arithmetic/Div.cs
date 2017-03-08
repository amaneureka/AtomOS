/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Div MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Assembler;
using Atomix.CompilerExt;
using System.Reflection;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Div)]
    public class ILDiv : MSIL
    {
        public ILDiv(Compiler Cmp)
            : base("div", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xItem = Core.vStack.Pop();
            var xSize = xItem.Size;

            /*
                value1 is pushed onto the stack.
                value2 is pushed onto the stack.
                value2 and value1 are popped from the stack;
                value1 is divided by value2.
                The result is pushed onto the stack.
            */
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        //Check the size of item first
                        if (xSize <= 4)
                        {
                            if (xItem.IsFloat)
                            {
                                //***What we are going to do is***
                                //1) Take the divisor value into XMM0 register
                                //2) Add ESP by 0x4 --> Like pop
                                //3) Take dividend value into XMM1 register
                                //4) Perform float division value1/value2
                                //5) Mov that value to 32 bit memory at ESP --> Like push
                                Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.XMM0, SourceReg = Registers.ESP, SourceIndirect = true });//value2 -> Divisor
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x4" });
                                Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.XMM1, SourceReg = Registers.ESP, SourceIndirect = true });//value1 -> Dividend
                                Core.AssemblerCode.Add(new Divss { DestinationReg = Registers.XMM1, SourceReg = Registers.XMM0 });
                                Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceReg = Registers.XMM1 });
                            }
                            else
                            {
                                //***What we are going to do is***
                                //1) Pop Divisor into EBX register
                                //2) Pop Dividend into EAX register
                                //3) Convert Dividend to 64 bit EDX:EAX
                                //4) Divide the content of EDX:EAX by EBX
                                //5) Push quotient in EAX to stack
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EBX });//value2 --> Divisor
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });//value1 --> Dividend
                                Core.AssemblerCode.Add(new Conversion { Type = ConversionCode.SignedDWord_2_SignedQWord});
                                Core.AssemblerCode.Add(new IDiv { DestinationReg = Registers.EBX });
                                Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                            }
                        }
                        else if (xSize <= 8)
                        {
                            if (xItem.IsFloat)
                            {
                                //***What we are going to do is***
                                //1) Load Dividend at 64 bit memory of ESP + 0x8 into floating stack
                                //2) Divide the floating stack content by 64bit memory at ESP
                                //3) Add ESP by 0x8 --> Just like Pop
                                //4) Pop Floating stack into 64 bit memory at ESP
                                Core.AssemblerCode.Add(new Fld { DestinationReg = Registers.ESP, DestinationIndirect = true, Size = 64, DestinationDisplacement = 0x8 });//value1 --> Dividend
                                Core.AssemblerCode.Add(new Fidiv { DestinationReg = Registers.ESP, DestinationIndirect = true, Size = 64 });//value2 --> Divisor
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x8" });
                                Core.AssemblerCode.Add(new Fstp { DestinationReg = Registers.ESP, DestinationIndirect = true, Size = 64 });
                            }
                            else
                            {
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
                                Core.AssemblerCode.Add(new Literal("inc dword ECX"));

                                // set flags
                                Core.AssemblerCode.Add(new Or { DestinationReg = Registers.EDI, SourceReg = Registers.EDI });
                                // loop while high dword of divisor till it is zero
                                Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNZ, DestinationRef = LabelShiftRight });

                                // shift the divident now in one step
                                // shift divident CL bits right
                                Core.AssemblerCode.Add(new Literal("shrd EAX, EDX, CL"));
                                Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.EDX, SourceReg = Registers.CL });

                                // so we shifted both, so we have near the same relation as original values
                                // divide this
                                Core.AssemblerCode.Add(new IDiv { DestinationReg = Registers.ESI });

                                // sign extend
                                Core.AssemblerCode.Add(new Conversion { Type = ConversionCode.SignedDWord_2_SignedQWord });

                                // save result to stack
                                Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EDX });
                                Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });

                                //TODO: implement proper derivation correction and overflow detection

                                Core.AssemblerCode.Add(new Jmp { DestinationRef = LabelEnd });

                                Core.AssemblerCode.Add(new Label(LabelNoLoop));
                                //save high dividend
                                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });
                                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EDX });
                                // extend that sign is in edx
                                Core.AssemblerCode.Add(new Conversion { Type = ConversionCode.SignedDWord_2_SignedQWord });
                                // divide high part
                                Core.AssemblerCode.Add(new IDiv { DestinationReg = Registers.ESI });
                                // save high result
                                Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ECX });
                                // divide low part
                                Core.AssemblerCode.Add(new Div { DestinationReg = Registers.ESI });
                                // save low result
                                Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });

                                Core.AssemblerCode.Add(new Label(LabelEnd));
                            }
                        }
                        else
                            //Division of more than size 8 is never called i guess :P
                            //But if it is called than will implement later
                            throw new Exception("@Div: Size greator than 8 is not yet supported");
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

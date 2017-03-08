/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Mul MSIL
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
    [ILOp(ILCode.Mul)]
    public class Mul : MSIL
    {
        public Mul(Compiler Cmp)
            : base("mul", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xItem = Core.vStack.Pop();
            var xSize = xItem.Size;

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        if (xSize <= 4)
                        {
                            if (xItem.IsFloat)
                            {
                                //***What we are going to do is***
                                //1) Take the value2 into XMM0 register
                                //2) Add ESP by 0x4 --> Like pop
                                //3) Take value1 into XMM1 register
                                //4) Perform float multiplication value1*value2
                                //5) Mov that value to 32 bit memory at ESP --> Like push
                                Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.XMM0, SourceReg = Registers.ESP, SourceIndirect = true });//value2
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x4" });
                                Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.XMM1, SourceReg = Registers.ESP, SourceIndirect = true });//value1
                                Core.AssemblerCode.Add(new Mulss { DestinationReg = Registers.XMM0, SourceReg = Registers.XMM1 });
                                Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceReg = Registers.XMM0 });
                            }
                            else
                            {
                                //***What we are going to do is***
                                //1) Pop value2 into EAX register
                                //2) Multiply EAX content with 32 bit memory at ESP and save result into EAX
                                //3) Add ESP by 0x4 --> Like pop
                                //4) Push Content of EAX into stack
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ESP, DestinationIndirect = true });
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x4" });
                                Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                            }
                        }
                        else if (xSize <= 8)
                        {
                            if (xItem.IsFloat)
                            {
                                //***What we are going to do is***
                                //1) Load 64 bit memory (value2) at ESP into floating stack
                                //2) Add ESP by 0x8 --> like pop
                                //3) Multiply 64 bit memory (value1) at ESP by content in floating stack
                                //4) Pop and store floating stack value into 64 bit memory at ESP --> Push
                                Core.AssemblerCode.Add(new Fld { DestinationReg = Registers.ESP, DestinationIndirect = true, Size = 64 });
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x8" });
                                Core.AssemblerCode.Add(new Fmul { DestinationReg = Registers.ESP, DestinationIndirect = true, Size = 64 });
                                Core.AssemblerCode.Add(new Fstp { DestinationReg = Registers.ESP, DestinationIndirect = true, Size = 64 });
                            }
                            else
                            {
                                // div of both == LEFT_LOW * RIGHT_LOW + ((LEFT_LOW * RIGHT_HIGH + RIGHT_LOW * LEFT_HIGH) << 32)
                                string BaseLabel = ILHelper.GetLabel(aMethod, instr.Position) + ".";
                                string Simple32Multiply = BaseLabel + "Simple32Multiply";
                                string MoveReturnValue = BaseLabel + "MoveReturnValue";

                                // right value
                                // low
                                //  SourceReg = Registers.ESP, SourceIndirect = true
                                // high
                                //  SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = 4

                                // left value
                                // low
                                //  SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = 8
                                // high
                                //  SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = 12

                                // compair LEFT_HIGH, RIGHT_HIGH , on zero only simple multiply is used
                                //mov RIGHT_HIGH to eax, is useable on Full 64 multiply
                                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = 4 });
                                Core.AssemblerCode.Add(new Or { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = 12 });
                                Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JZ, DestinationRef = Simple32Multiply });
                                // Full 64 Multiply

                                // copy again, or could change EAX
                                //TODO is there an opcode that does OR without change EAX?
                                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true, SourceDisplacement = 4 });
                                // eax contains already RIGHT_HIGH
                                // multiply with LEFT_LOW
                                Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ESP, DestinationIndirect = true, DestinationDisplacement = 8, Size = 32 });
                                // save result of LEFT_LOW * RIGHT_HIGH
                                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });

                                //mov RIGHT_LOW to eax
                                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true });
                                // multiply with LEFT_HIGH
                                Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ESP, DestinationIndirect = true, DestinationDisplacement = 12, Size = 32 });
                                // add result of LEFT_LOW * RIGHT_HIGH + RIGHT_LOW + LEFT_HIGH
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });

                                //mov RIGHT_LOW to eax
                                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true });
                                // multiply with LEFT_LOW
                                Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ESP, DestinationIndirect = true, DestinationDisplacement = 8, Size = 32 });
                                // add LEFT_LOW * RIGHT_HIGH + RIGHT_LOW + LEFT_HIGH to high dword of last result
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EDX, SourceReg = Registers.ECX });

                                Core.AssemblerCode.Add(new Jmp { DestinationRef = MoveReturnValue });

                                Core.AssemblerCode.Add(new Label(Simple32Multiply));
                                //mov RIGHT_LOW to eax
                                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESP, SourceIndirect = true });
                                // multiply with LEFT_LOW
                                Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ESP, DestinationIndirect = true, DestinationDisplacement = 8, Size = 32 });

                                Core.AssemblerCode.Add(new Label(MoveReturnValue));
                                // move high result to left high
                                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESP, DestinationIndirect = true, DestinationDisplacement = 12, SourceReg = Registers.EDX });
                                // move low result to left low
                                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESP, DestinationIndirect = true, DestinationDisplacement = 8, SourceReg = Registers.EAX });
                                // pop right 64 value
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x8" });
                            }
                        }
                        else
                            //Multiplication of more than size 8 is never called i guess :P
                            //But if it is called than will implement later
                            throw new Exception("@Mul: Size greator than 8 is not yet supported");
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

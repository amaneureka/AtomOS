/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Sub MSIL
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
    [ILOp(ILCode.Sub)]
    public class ILSub : MSIL
    {
        public ILSub(Compiler Cmp)
            : base("sub", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //We pop first content and peek second because we just want size of content
            var xItem = Core.vStack.Pop();
            var xSize = xItem.Size;

            //If stack size of operands are not same, than we can't do Sub operation
            if (xSize != Core.vStack.Peek().Size)
                throw new Exception("@Sub: different size in subtraction is not possible :(; Sizes=" + xSize + ", " + Core.vStack.Peek().Size);

            /*
                value1 is pushed onto the stack.
                value2 is pushed onto the stack.
                value2 and value1 are popped from the stack;
                value2 is subtracted from value1.
                The result is pushed onto the stack.
            */

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
                                //1) Copy ESP value 2 to XMM0 register
                                //2) Add ESP by 4 to move stack pointer
                                //3) Copy ESP value 1 to XMM1
                                //4) Sub both values are take result to XMM1 --> XMM1 = XMM1 - XMM2
                                //5) Copy the value to memory at ESP --> Just like push
                                Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.XMM0, SourceReg = Registers.ESP, SourceIndirect = true });//1st value
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x4" });
                                Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.XMM1, SourceReg = Registers.ESP, SourceIndirect = true });//2nd value
                                Core.AssemblerCode.Add(new Subss { DestinationReg = Registers.XMM1, SourceReg = Registers.XMM0 });
                                Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceReg = Registers.XMM1 });
                            }
                            else
                            {
                                //***What we are going to do is***
                                //1) Pop the value 2 from the stack into EBX
                                //2) Pop the value 1 from the stack into EAX
                                //3) Sub EBX from EAX and save value into EAX
                                //4) Push content of EAX to stack
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EBX });
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.EAX, SourceReg = Registers.EBX });
                                Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                            }
                        }
                        else if (xSize <= 8)
                        {
                            if (xItem.IsFloat)
                            {
                                //***What we are going to do is***
                                //1) Load the 64 bit memory at ESP + 0x8 into FLD stack
                                //2) Use Float Sub to add value in float stack to 64 bit memory at ESP, And store result in float stack
                                //3) Add ESP by 0x8 --> To shift pointer
                                //4) Just like Pop, we pop the iteam from float stack and put it into 64 bit memory at ESP
                                Core.AssemblerCode.Add(new Fld { DestinationReg = Registers.ESP, DestinationIndirect = true, Size = 64, DestinationDisplacement = 0x8 });
                                Core.AssemblerCode.Add(new Fsub { DestinationReg = Registers.ESP, DestinationIndirect = true, Size = 64 });
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x8" });
                                Core.AssemblerCode.Add(new Fstp { DestinationReg = Registers.ESP, DestinationIndirect = true, Size = 64 });
                            }
                            else
                            {
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EDX });

                                Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceReg = Registers.EAX });
                                Core.AssemblerCode.Add(new SubWithCarry { DestinationReg = Registers.ESP, DestinationIndirect = true, DestinationDisplacement = 4, SourceReg = Registers.EDX });
                            }
                        }
                        else
                            //Subtraction of more than size 8 is never called i guess :P
                            //But if it is called than will implement later
                            throw new Exception("@Sub: Size greator than 8 is not yet supported");
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

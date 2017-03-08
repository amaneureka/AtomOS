/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Add MSIL
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
    [ILOp(ILCode.Add)]
    public class ILAdd : MSIL
    {
        public ILAdd(Compiler Cmp)
            : base("add", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //Tell virtual stack that we want to pop
            var xItem = Core.vStack.Pop();
            //Get Size of pop content and align it to 4, i mean multiple of 4
            var xSize = xItem.Size;

            /*
                    The stack transitional behavior, in sequential order, is:
                    value1 is pushed onto the stack.
                    value2 is pushed onto the stack.
                    value2 and value1 are popped from the stack; value1 is added to value2.
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
                                //So, we have again both floating point in stack
                                //http://rayseyfarth.com/asm/pdf/ch11-floating-point.pdf#6

                                //***What we are going to do is***
                                //1) Copy ESP value 2 to XMM0 register
                                //2) Add ESP by 4 to move stack pointer
                                //3) Copy ESP value 1 to XMM1
                                //4) Add both values are take result to XMM0
                                //5) Copy the value to memory at ESP --> Just like push
                                Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.XMM0, SourceReg = Registers.ESP, SourceIndirect = true });
                                Core.AssemblerCode.Add(new Assembler.x86.Add { DestinationReg = Registers.ESP, SourceRef = "0x4" });
                                Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.XMM1, SourceReg = Registers.ESP, SourceIndirect = true });
                                Core.AssemblerCode.Add(new Addss { DestinationReg = Registers.XMM0, SourceReg = Registers.XMM1 });
                                Core.AssemblerCode.Add(new Movss { DestinationReg = Registers.ESP, SourceReg = Registers.XMM0, DestinationIndirect = true });
                            }
                            else
                            {
                                //***What we are going to do is***
                                //1) Pop the value 2 from the stack into EAX
                                //2) Add the content of EAX to the 32bit memory at ESP
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceReg = Registers.EAX, DestinationIndirect = true });
                            }
                        }
                        else if (xSize <= 8)
                        {
                            if (xItem.IsFloat)
                            {
                                //I got the idea of this implementation from GCC, i build simple code and decompile it with gcc
                                // and look at its assembly code
                                //      a+=b;
                                //18:   dd 45 f8                fld    QWORD PTR [ebp-0x8]
                                //1b:   dc 45 f0                fadd   QWORD PTR [ebp-0x10]
                                //1e:   dd 5d f8                fstp   QWORD PTR [ebp-0x8]

                                //***What we are going to do is***
                                //1) Load the 64 bit memory at ESP into FLD stack
                                //2) Add ESP by 0x8 --> To shift pointer
                                //3) Use Float Add to add value in float stack to 64 bit memory at ESP, And store result in float stack
                                //4) Just like Pop, we pop the iteam from float stack and put it into 64 bit memory at ESP
                                Core.AssemblerCode.Add(new Fld { DestinationReg = Registers.ESP, Size = 64, DestinationIndirect = true });
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x8" });
                                Core.AssemblerCode.Add(new Fadd { DestinationReg = Registers.ESP, Size = 64, DestinationIndirect = true });
                                Core.AssemblerCode.Add(new Fstp { DestinationReg = Registers.ESP, Size = 64, DestinationIndirect = true });
                            }
                            else
                            {
                                //Two Qword parameters are in stack, so first pop it into edx:eax and add it to [ESP + 4] : [ESP] with carry
                                //http://stackoverflow.com/questions/7865511/qword-storing-implementing-using-32-bit-regs

                                //***What we are going to do is***
                                //1) Pop the low part of value 2 into EAX
                                //2) Pop the high part of value 2 into EDX
                                //3) Add the EAX to 32 bit Memory at ESP --> Low part of value 1
                                //4) Add the EDX to 32 bit Memory at ESP + 4 with carry --> High part of value 1
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX }); //low part
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EDX }); //High part
                                Core.AssemblerCode.Add(
                                    new Assembler.x86.Add
                                    {
                                        DestinationReg = Registers.ESP,
                                        SourceReg = Registers.EAX,
                                        DestinationIndirect = true
                                    });

                                Core.AssemblerCode.Add(
                                    new AddWithCarry
                                    {
                                        DestinationReg = Registers.ESP,
                                        SourceReg = Registers.EDX,
                                        DestinationIndirect = true,
                                        DestinationDisplacement = 4
                                    });
                            }
                        }
                        else
                            //Add of more than size 8 is never called i guess :P
                            //But if it is called than will implement later
                            throw new Exception("@Add: Size greator than 8 is not yet supported");
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

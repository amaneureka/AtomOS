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
    [ILOp(ILCode.Div_Un)]
    public class Div_Un : MSIL
    {
        public Div_Un(Compiler Cmp)
            : base("div_un", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //We want both items but we are peeking the second because we have to push it again
            //so making two lines of more code is difficult for me hence i peek it :P
            var xItem = Core.vStack.Pop();
            //Calculate the maximum size first
            var xSize = Math.Max(xItem.Size, Core.vStack.Peek().Size);

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
                                //3) XOR EDX Don't use cdq { http://www.asmcommunity.net/forums/topic/?id=9684 }
                                //4) Divide the content of EDX:EAX by EBX
                                //5) Push quotient in EAX to stack
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EBX });//value2 --> Divisor
                                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });//value1 --> Dividend
                                Core.AssemblerCode.Add(new Xor { DestinationReg = Registers.EDX, SourceReg =Registers.EDX });
                                Core.AssemblerCode.Add(new Div { DestinationReg = Registers.EBX });
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
                                Core.AssemblerCode.Add(new Fdiv { DestinationReg = Registers.ESP, DestinationIndirect = true, Size = 64 });//value2 --> Divisor
                                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x8" });
                                Core.AssemblerCode.Add(new Fstp { DestinationReg = Registers.ESP, DestinationIndirect = true, Size = 64 });
                            }
                            else
                                //Yet to Implement man
                                throw new Exception("@Div_Un: Need to implement division of integer size 8");
                        }
                        else
                            //Division of more than size 8 is never called i guess :P
                            //But if it is called than will implement later
                            throw new Exception("@Div_Un: Size greator than 8 is not yet supported");
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

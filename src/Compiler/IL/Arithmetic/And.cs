/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          And MSIL
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
    [ILOp(ILCode.And)]
    public class ILAnd : MSIL
    {
        public ILAnd(Compiler Cmp)
            : base("and", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //We pop first content and peek second because we just want what's the maximum size of content
            var xSize1 = Core.vStack.Pop().Size;
            var xSize2 = Core.vStack.Peek().Size;
            var xSize = Math.Max(xSize1, xSize2);

            /*
                value1 is pushed onto the stack.
                value2 is pushed onto the stack.
                value1 and value2 are popped from the stack; the bitwise AND of the two values is computed.
                The result is pushed onto the stack.
            */

            //If stack size of operands are not same, than we can't do And operation
            if (xSize1.Align() != xSize2.Align())
                throw new Exception("@And: Size of operands are different");

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        if (xSize <= 4)
                        {
                            //***What we are going to do is***
                            //1) Pop value 2 into EAX
                            //2) Perform And Operation on 32 bit memory at ESP to EAX
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                            Core.AssemblerCode.Add(new And { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceReg = Registers.EAX });
                        }
                        else if (xSize <= 8)
                        {
                            //***What we are going to do is***
                            //1) Pop Low part of value 2 into EAX
                            //2) Pop High part of value 1 into EDX
                            //3) Perform And operation of EAX and 32 bit memory at ESP
                            //4) Perform And operation of EAX and 32 bit memory at ESP + 0x4
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });//low part
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EDX });//high part

                            Core.AssemblerCode.Add(new And { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceReg = Registers.EAX });
                            Core.AssemblerCode.Add(new And { DestinationReg = Registers.ESP, DestinationIndirect = true, SourceReg = Registers.EDX, DestinationDisplacement = 0x4 });
                        }
                        else
                            //Case of And operation more than 8 is not usually called
                            throw new Exception("@And: bitwise and operation more than size 8 is not yet supported");
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

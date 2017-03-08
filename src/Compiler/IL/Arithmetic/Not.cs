/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Not MSIL
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
    [ILOp(ILCode.Not)]
    public class ILNot : MSIL
    {
        public ILNot(Compiler Cmp)
            : base("not", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //We need size only
            var xSize = Core.vStack.Peek().Size;

            /*
                value is pushed onto the stack.
                value is popped from the stack and its bitwise complement computed.
                The result is pushed onto the stack.
            */
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        if (xSize <= 4)
                        {
                            //***What we are going to do is***
                            //1) Pop value 2 into EAX
                            //2) Perform not Operation on EAX
                            //3) Push EAX
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Not { DestinationReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
                        }
                        else if (xSize <= 8)
                        {
                            //***What we are going to do is***
                            //1) Pop Low part of value 2 into EAX
                            //2) Pop High part of value 1 into EDX
                            //3) Perform not Operation on EAX
                            //4) Perform not Operation on EDX
                            //5) Push EDX --> High part first
                            //6) Push EAX
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });//low part
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EDX });//high part

                            Core.AssemblerCode.Add(new Not { DestinationReg = Registers.EAX });
                            Core.AssemblerCode.Add(new Not { DestinationReg = Registers.EDX });

                            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EDX });//high part
                            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });//low part
                        }
                        else
                            //Case of And operation more than 8 is not usually called
                            throw new Exception("@Not: bitwise and operation more than size 8 is not yet supported");
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

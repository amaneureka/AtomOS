/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Box MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Box)]
    public class Box : MSIL
    {
        public Box(Compiler Cmp)
            : base("box", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xOpType = ((OpType)instr).Value;
            var xSize = xOpType.SizeOf().Align();
            var xTypeID = ILHelper.GetTypeID(xOpType);

            /*
                A value type is pushed onto the stack.
                The value type is popped from the stack; the box operation is performed.
                An object reference to the resulting "boxed" value type is pushed onto the stack.
            */
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        #warning Have to check memory allocation here, so don't use it now
                        //Why i did this? well box is nothing but it converts object type so, lets assume it is already what we want :P
                        Console.WriteLine("Box Operation is being called by " + aMethod.FullName() + "\n" + xOpType);
                        break;
                        throw new Exception("Not yet implemented");
                        //***What we are going to do is***
                        //1) Push the size of object + 0xC --> The 0xC is the offset of object data before this object metadata is stored
                        //2) Call our memory manager
                        //3) After that we have done boxing :P
                        Core.AssemblerCode.Add(new Push { DestinationRef = "0x" + (0xC + xSize).ToString("X") });
                        Core.AssemblerCode.Add(new Call (Helper.lblHeap, true));
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, SourceRef = "0x" + xTypeID.ToString("X") });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = 4, SourceRef = "0x3" });

                        for (int i = 0; i < (xSize / 4); i++)
                        {
                            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EDX });
                            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = (0xC + (i * 4)), SourceReg = Registers.EDX });
                        }
                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
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

            Core.vStack.Pop();
            Core.vStack.Push(4, typeof(UIntPtr));
        }
    }
}

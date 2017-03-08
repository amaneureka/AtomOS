/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Newarr MSIL
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
    [ILOp(ILCode.Newarr)]
    public class Newarr : MSIL
    {
        public Newarr(Compiler Cmp)
            : base("newarr", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xOperand = ((OpType)instr);
            var xTargetType = xOperand.Value;
            //Size of one entry of array i.e. Size_of_one_entry.
            var xSize = xTargetType.SizeOf();

            var xTypeID = ILHelper.GetTypeID(typeof(Array));

            //HACK: this is a kind of hack :D
            var xArray_ctor = typeof(Array).GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)[0];
            var xArray_ctor_label = xArray_ctor.FullName();

            Compiler.QueuedMember.Enqueue(xArray_ctor);

            //Number of elements
            Core.vStack.Pop();

            /*
                The number of elements in the array is pushed onto the stack.
                The number of elements is popped from the stack and the array is created.
                An object reference to the new array is pushed onto the stack.
            */
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESI, SourceReg = Registers.ESP, SourceIndirect = true });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceRef = "0x" + xSize.ToString("X") });
                        Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ESI });
                        Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceRef = "0x10" });
                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });

                        //Call our Heap
                        Core.AssemblerCode.Add(new Call(Helper.lblHeap, true));

                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });//Address
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.ESI });//Number of Elements

                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, SourceRef = "0x" + xTypeID.ToString("X") });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = 4, SourceRef = "0x2" });//Array Signature here 0x2
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = 8, SourceReg = Registers.ESI });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = 12, SourceRef = "0x" + xSize.ToString("X") });

                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });//For final array refernce
                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });//For ctor
                        Core.AssemblerCode.Add(new Call(xArray_ctor_label));
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
            Core.vStack.Push(4, typeof(Array));
        }
    }
}

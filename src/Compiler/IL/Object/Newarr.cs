using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.IL;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using System.Reflection;
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
            var xHeap = (Core.StaticLabels["Heap"] as MethodBase).FullName();
            
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
                        /*
                         * OLD Code
                        //***What we are going to do is***
                        //1) We calculate total size of array that is 0xC + 0x4 + (Size_of_one_entry * Total_entries)
                        //      a) Pop number of elements into ESI
                        //      b) Mov ESI to EAX so we have two copies of value "Total_entries"
                        //      c) Set EBX = Size_of_one_entry
                        //      d) EAX *= EBX
                        //      e) EAX += 0x10, Hence we have Total Size in register EAX so push it and call memory allocator
                        //2) Call our memory manager and take the address
                        //3) Pop the address in EAX
                        //4) Write Signature, length and size of type to that address and call Array
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.ESI });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.ESI });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceRef = "0x" + xSize.ToString("X") });
                        Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EBX });
                        Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceRef = "0x10" });
                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });//<-- only this is the stack
                        */
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.ESI });
                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.ESI });
                        Core.AssemblerCode.Add(new Push { DestinationRef = "0x" + xSize.ToString("X") });
                        Core.vStack.Push(4, typeof(UInt32));

                        Compiler.MSIL[ILCode.Mul].Execute(instr, aMethod);

                        Core.AssemblerCode.Add(new Push { DestinationRef = "0x10" });
                        Core.vStack.Push(4, typeof(UInt32));

                        Compiler.MSIL[ILCode.Add].Execute(instr, aMethod);

                        //Call our Heap
                        Core.AssemblerCode.Add(new Call(xHeap));
                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.ESP, DestinationIndirect = true });
                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.ESP, DestinationIndirect = true });

                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceRef = "0x" + xTypeID.ToString("X") });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, SourceReg = Registers.EBX });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = 4, SourceRef = "0x2" });//Array Signature here 0x2
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = 8, SourceReg = Registers.ESI });
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, DestinationIndirect = true, DestinationDisplacement = 12, SourceRef = "0x" + xSize.ToString("X") });

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

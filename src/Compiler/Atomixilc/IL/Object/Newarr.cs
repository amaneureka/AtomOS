/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Newarr MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Newarr)]
    internal class Newarr_il : MSIL
    {
        public Newarr_il()
            : base(ILCode.Newarr)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Newarr(v=vs.110).aspx
         * Description : Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 1");

            var type = ((OpType)xOp).Value;
            var size = Helper.GetTypeSize(type, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * The number of elements in the array is pushed onto the stack.
             * The number of elements is popped from the stack and the array is created.
             * An object reference to the new array is pushed onto the stack.
             */

            var item = Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (!item.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        new Mov { DestinationReg = Register.ESI, SourceReg = Register.ESP, SourceIndirect = true };
                        new Mov { DestinationReg = Register.EAX, SourceRef = "0x" + size.ToString() };
                        new Mul { DestinationReg = Register.ESI };
                        new Add { DestinationReg = Register.EAX, SourceRef = "0x10" };

                        new Push { DestinationReg = Register.EAX };
                        new Call { DestinationRef = Helper.Heap_Label, IsLabel = true };
                        new Test { DestinationReg = Register.ECX, SourceRef = "0xFFFFFFFF" };
                        new Jmp { Condition = ConditionalJump.JNZ, DestinationRef = xOp.HandlerRef };

                        new Pop { DestinationReg = Register.ESI };
                        new Mov { DestinationReg = Register.EAX, DestinationIndirect = true, SourceRef = "0x" + type.GetHashCode().ToString("X") };
                        new Mov { DestinationReg = Register.EAX, DestinationIndirect = true, DestinationDisplacement = 4, SourceRef = "0x2" };
                        new Mov { DestinationReg = Register.EAX, DestinationIndirect = true, DestinationDisplacement = 8, SourceReg = Register.ESI };
                        new Mov { DestinationReg = Register.EAX, DestinationIndirect = true, DestinationDisplacement = 12, SourceRef = "0x" + size.ToString("X") };

                        new Push { DestinationReg = Register.EAX };

                        Optimizer.vStack.Push(new StackItem(typeof(Array)));
                        Optimizer.SaveStack(xOp.NextPosition);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldelem)]
    internal class Ldelem_il : MSIL
    {
        public Ldelem_il()
            : base(ILCode.Ldelem)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldelem(v=vs.110).aspx
         * Description : Loads the element at a specified array index onto the top of the evaluation stack as the type specified in the instruction.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 2)
                throw new Exception("Internal Compiler Error: vStack.Count < 2");

            var operand = ((OpType)xOp).Value;
            var size = Helper.GetTypeSize(operand, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * An object reference array is pushed onto the stack.
             * An index value index is pushed onto the stack.
             * index and array are popped from the stack; the value stored at position index in array is looked up.
             * The value is pushed onto the stack.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            var itemA = Optimizer.vStack.Pop();
            var itemB = Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (size > 4)
                            throw new Exception(string.Format("UnImplemented '{0}'", msIL));

                        if (!itemA.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        if (!itemB.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        Executex86(size, true);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.vStack.Push(new StackItem(operand));
        }

        internal static void Executex86(int size, bool IsSigned)
        {
            new Pop { DestinationReg = Register.EAX };
            new Mov { DestinationReg = Register.EDX, SourceRef = "0x" + size.ToString("X") };
            new Mul { DestinationReg = Register.EDX };

            new Pop { DestinationReg = Register.EDX };
            new Add { DestinationReg = Register.EDX, SourceReg = Register.EAX };

            if (IsSigned)
                new Movsx { DestinationReg = Register.EAX, SourceReg = Register.EDX, SourceDisplacement = 0x10, SourceIndirect = true, Size = (byte)(size * 8) };
            else
                new Movzx { DestinationReg = Register.EAX, SourceReg = Register.EDX, SourceDisplacement = 0x10, SourceIndirect = true, Size = (byte)(size * 8) };
            new Push { DestinationReg = Register.EAX };
        }
    }
}

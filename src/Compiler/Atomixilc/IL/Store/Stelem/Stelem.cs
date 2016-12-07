using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Stelem)]
    internal class Stelem_il : MSIL
    {
        public Stelem_il()
            : base(ILCode.Stelem)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Stelem(v=vs.110).aspx
         * Description : Replaces the array element at a given index with the value on the evaluation stack, whose type is specified in the instruction.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 3)
                throw new Exception("Internal Compiler Error: vStack.Count < 3");

            var operand = ((OpType)xOp).Value;
            var size = Helper.GetTypeSize(operand, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * An object reference to an array, array, is pushed onto the stack.
             * An index value, index, to an element in array is pushed onto the stack.
             * A value of the type specified in the instruction is pushed onto the stack.
             * The value, the index, and the array reference are popped from the stack; the value is put into the array element at the given index.
             */

            Optimizer.vStack.Pop();
            Optimizer.vStack.Pop();
            Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (size > 4)
                            throw new Exception("LocalVariable size > 4 not supported");

                        Executex86(size);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }

        internal static void Executex86(int size)
        {
            new Pop { DestinationReg = Register.EDI };
            new Pop { DestinationReg = Register.EAX };
            new Mov { DestinationReg = Register.EDX, SourceRef = "0x" + size.ToString("X") };
            new Mul { DestinationReg = Register.EDX };

            new Pop { DestinationReg = Register.EDX };
            new Add { DestinationReg = Register.EDX, SourceReg = Register.EAX };

            new Mov
            {
                DestinationReg = Register.EDX,
                DestinationDisplacement = 0x10,
                DestinationIndirect = true,
                SourceReg = Register.EDI,
                Size = (byte)(size * 8)
            };
        }
    }
}

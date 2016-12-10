using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.IL.CodeType;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Cgt_Un)]
    internal class Cgt_Un_il : MSIL
    {
        public Cgt_Un_il()
            : base(ILCode.Cgt_Un)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Cgt_Un(v=vs.110).aspx
         * Description : Compares two unsigned or unordered values. If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 2)
                throw new Exception("Internal Compiler Error: vStack.Count < 2");

            var itemA = Optimizer.vStack.Pop();
            var itemB = Optimizer.vStack.Pop();

            var xCurrentLabel = Helper.GetLabel(xOp.Position);
            var xNextLabel = Helper.GetLabel(xOp.NextPosition);

            var size = Math.Max(Helper.GetTypeSize(itemA.OperandType, Config.TargetPlatform),
                Helper.GetTypeSize(itemB.OperandType, Config.TargetPlatform));

            /* The stack transitional behavior, in sequential order, is:
             * value1 is pushed onto the stack.
             * value2 is pushed onto the stack.
             * value2 and value1 are popped from the stack; cgt.un tests if value1 is greater than value2.
             * If value1 is greater than value2, 1 is pushed onto the stack; otherwise 0 is pushed onto the stack.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (itemA.IsFloat || itemB.IsFloat || size > 4)
                            throw new Exception(string.Format("UnImplemented '{0}'", msIL));

                        if (!itemA.SystemStack || !itemB.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        new Pop { DestinationReg = Register.EDX };
                        new Pop { DestinationReg = Register.EAX };
                        new Cmp { DestinationReg = Register.EAX, SourceReg = Register.EDX };
                        new Jmp { Condition = ConditionalJump.JA, DestinationRef = xCurrentLabel + ".true" };

                        new Push { DestinationRef = "0x0" };
                        new Jmp { DestinationRef = xNextLabel };

                        new Label(xCurrentLabel + ".true");
                        new Push { DestinationRef = "0x1" };

                        new Label(xNextLabel);

                        Optimizer.vStack.Push(new StackItem(typeof(bool)));
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

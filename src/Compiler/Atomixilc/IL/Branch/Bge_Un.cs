using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.IL.CodeType;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Bge_Un)]
    internal class Bge_Un_il : MSIL
    {
        public Bge_Un_il()
            : base(ILCode.Bge_Un)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Bge_Un(v=vs.110).aspx
         * Description : Transfers control to a target instruction if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 2)
                throw new Exception("Internal Compiler Error: vStack.Count < 2");

            var offset = ((OpBranch)xOp).Value;
            var itemA = Optimizer.vStack.Pop();
            var itemB = Optimizer.vStack.Pop();

            var xTrueLabel = Helper.GetLabel(offset);

            var size = Math.Max(Helper.GetTypeSize(itemA.OperandType, Config.TargetPlatform),
                Helper.GetTypeSize(itemB.OperandType, Config.TargetPlatform));

            /* The stack transitional behavior, in sequential order, is:
             * value1 is pushed onto the stack.
             * value2 is pushed onto the stack.
             * value2 and value1 are popped from the stack; if value1 is greater than or equal to value2, the branch operation is performed.
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
                        new Jmp { Condition = ConditionalJump.JAE, DestinationRef = xTrueLabel };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

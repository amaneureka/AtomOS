/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Brtrue MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.IL.CodeType;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Brtrue)]
    internal class Brtrue_il : MSIL
    {
        public Brtrue_il()
            : base(ILCode.Brtrue)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Brtrue(v=vs.110).aspx
         * Description : Transfers control to a target instruction if value is true, not null, or non-zero.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 1");

            var offset = ((OpBranch)xOp).Value;
            var item = Optimizer.vStack.Pop();

            var xTrueLabel = Helper.GetLabel(offset);

            var size = Helper.GetTypeSize(item.OperandType, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * value is pushed onto the stack by a previous operation.
             * value is popped from the stack; if value is true, branch to target.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (item.IsFloat || size > 4)
                            throw new Exception(string.Format("UnImplemented '{0}'", msIL));

                        if (!item.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        new Pop { DestinationReg = Register.EAX };
                        new Cmp { DestinationReg = Register.EAX, SourceRef = "0x0" };
                        new Jmp { Condition = ConditionalJump.JNZ, DestinationRef = xTrueLabel };

                        Optimizer.SaveStack(offset);
                        Optimizer.SaveStack(xOp.NextPosition);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

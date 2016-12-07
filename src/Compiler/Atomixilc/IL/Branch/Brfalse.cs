using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.IL.CodeType;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Brfalse)]
    internal class Brfalse_il : MSIL
    {
        public Brfalse_il()
            : base(ILCode.Brfalse)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Brfalse(v=vs.110).aspx
         * Description : Transfers control to a target instruction if value is false, a null reference (Nothing in Visual Basic), or zero.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 1");

            var offset = ((OpBranch)xOp).Value;
            var item = Optimizer.vStack.Pop();

            var xTrueLabel = Helper.GetLabel(method, offset);

            var size = Helper.GetTypeSize(item.OperandType, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * value is pushed onto the stack by a previous operation.
             * value is popped from the stack; if value is false, branch to target.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

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
                        new Jmp { Condition = ConditionalJump.JZ, DestinationRef = xTrueLabel };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

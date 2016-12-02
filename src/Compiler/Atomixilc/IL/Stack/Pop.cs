using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Pop)]
    internal class Pop_il : MSIL
    {
        internal Pop_il()
            : base(ILCode.Pop)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Pop(v=vs.110).aspx
         * Description : Removes the value currently on top of the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 2");

            Optimizer.vStack.Pop();

            /* The stack transitional behavior, in sequential order, is:
             * The top value is popped from the stack.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        new Add { DestinationReg = Register.ESP, SourceRef = "0x4" };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

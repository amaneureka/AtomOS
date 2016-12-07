using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldc_I4)]
    internal class Ldc_I4_il : MSIL
    {
        public Ldc_I4_il()
            : base(ILCode.Ldc_I4)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldc_I4(v=vs.110).aspx
         * Description : Pushes a supplied value of type int32 onto the evaluation stack as an int32.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var num = ((OpInt)xOp).Value;

            /* The stack transitional behavior, in sequential order, is:
             * The value num is pushed onto the stack
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        new Push { DestinationRef = "0x" + num.ToString("X") };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.vStack.Push(new StackItem(typeof(int)));
        }
    }
}

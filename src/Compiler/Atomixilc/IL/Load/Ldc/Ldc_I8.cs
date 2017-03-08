/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldc_I8 MSIL
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
    [ILImpl(ILCode.Ldc_I8)]
    internal class Ldc_I8_il : MSIL
    {
        public Ldc_I8_il()
            : base(ILCode.Ldc_I8)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldc_I8(v=vs.110).aspx
         * Description : Pushes a supplied value of type int64 onto the evaluation stack as an int64.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var num = ((OpInt64)xOp).Value;

            /* The stack transitional behavior, in sequential order, is:
             * The value num is pushed onto the stack
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        new Push { DestinationRef = "0x" + ((uint)(num >> 32)).ToString("X") };
                        new Push { DestinationRef = "0x" + ((uint)num).ToString("X") };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.vStack.Push(new StackItem(typeof(long)));
            Optimizer.SaveStack(xOp.NextPosition);
        }
    }
}

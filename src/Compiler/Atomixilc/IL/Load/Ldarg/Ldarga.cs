/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldarga MSIL
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
    [ILImpl(ILCode.Ldarga)]
    internal class Ldarga_il : MSIL
    {
        public Ldarga_il()
            : base(ILCode.Ldarga)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldarga(v=vs.110).aspx
         * Description : Load an argument address onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var index = ((OpVar)xOp).Value;

            int EBPoffset = Ldarg_il.GetArgumentOffset(Config, method, index);

            /* The stack transitional behavior, in sequential order, is:
             * The address addr of the argument indexed by index is pushed onto the stack.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        new Lea { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = EBPoffset, SourceIndirect = true };
                        new Push { DestinationReg = Register.EAX };

                        Optimizer.vStack.Push(new StackItem(typeof(uint)));
                        Optimizer.SaveStack(xOp.NextPosition);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

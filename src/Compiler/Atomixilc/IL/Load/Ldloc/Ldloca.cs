/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldloca MSIL
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
    [ILImpl(ILCode.Ldloca)]
    internal class Ldloca_il : MSIL
    {
        public Ldloca_il()
            : base(ILCode.Ldloca)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldloca(v=vs.110).aspx
         * Description : Loads the address of the local variable at a specific index onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var index = ((OpVar)xOp).Value;

            var body = method.GetMethodBody();
            var EBPoffset = Helper.GetVariableOffset(body, index, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * The address stored in the local variable at the specified index is pushed onto the stack.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        new Lea { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = -EBPoffset, SourceIndirect = true };
                        new Push { DestinationReg = Register.EAX };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.vStack.Push(new StackItem(typeof(uint)));
            Optimizer.SaveStack(xOp.NextPosition);
        }
    }
}

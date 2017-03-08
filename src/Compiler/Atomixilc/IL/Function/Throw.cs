/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Throw MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Throw)]
    internal class Throw_il : MSIL
    {
        public Throw_il()
            : base(ILCode.Throw)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Throw(v=vs.110).aspx
         * Description : Throws the exception object currently on the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 1");

            var item = Optimizer.vStack.Pop();

            /* The stack transitional behavior, in sequential order, is:
             * An object reference (to an exception) is pushed onto the stack.
             * The object reference is popped from the stack and the exception thrown.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (!item.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        new Pop { DestinationReg = Register.ECX };
                        new Jmp { DestinationRef = xOp.HandlerRef };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

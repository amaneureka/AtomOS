/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldind_U2 MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldind_U2)]
    internal class Ldind_U2_il : MSIL
    {
        public Ldind_U2_il()
            : base(ILCode.Ldind_U2)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldind_U2(v=vs.110).aspx
         * Description : Loads a value of type unsigned int16 as an int32 onto the evaluation stack indirectly.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 1");

            /* The stack transitional behavior, in sequential order, is:
             * An address is pushed onto the stack.
             * The address is popped from the stack; the value located at the address is fetched.
             * The fetched value is pushed onto the stack.
             */

            var item = Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (!item.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        Ldind_I_il.Executex86(2, false);
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

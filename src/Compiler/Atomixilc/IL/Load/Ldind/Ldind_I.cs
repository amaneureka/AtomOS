/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldind_I MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldind_I)]
    internal class Ldind_I_il : MSIL
    {
        public Ldind_I_il()
            : base(ILCode.Ldind_I)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldind_I(v=vs.110).aspx
         * Description : Loads a value of type native int as a native int onto the evaluation stack indirectly.
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

                        Executex86(4, true);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.vStack.Push(new StackItem(typeof(int)));
            Optimizer.SaveStack(xOp.NextPosition);
        }

        internal static void Executex86(int size, bool IsSigned)
        {
            new Pop { DestinationReg = Register.EDX };

            if (size < 4)
            {
                if (IsSigned)
                    new Movsx { DestinationReg = Register.EAX, SourceReg = Register.EDX, SourceIndirect = true, Size = (byte)(size * 8) };
                else
                    new Movzx { DestinationReg = Register.EAX, SourceReg = Register.EDX, SourceIndirect = true, Size = (byte)(size * 8) };
                new Push { DestinationReg = Register.EAX };
            }
            else
                new Push { DestinationReg = Register.EDX, DestinationIndirect = true };
        }
    }
}

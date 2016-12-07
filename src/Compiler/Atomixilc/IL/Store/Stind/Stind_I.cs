using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Stind_I)]
    internal class Stind_I_il : MSIL
    {
        public Stind_I_il()
            : base(ILCode.Stind_I)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Stind_I(v=vs.110).aspx
         * Description : Stores a value of type native int at a supplied address.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 2)
                throw new Exception("Internal Compiler Error: vStack.Count < 2");

            /* The stack transitional behavior, in sequential order, is:
             * An address is pushed onto the stack.
             * A value is pushed onto the stack.
             * The value and the address are popped from the stack; the value is stored at the address.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            Optimizer.vStack.Pop();
            Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        Executex86(4);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }

        internal static void Executex86(int size)
        {
            new Pop { DestinationReg = Register.EDX };
            new Pop { DestinationReg = Register.EAX };

            new Mov
            {
                DestinationReg = Register.EAX,
                DestinationIndirect = true,
                SourceReg = Register.EDX,
                Size = (byte)(size * 8)
            };
        }
    }
}

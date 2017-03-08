/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stind_I MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

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

            var itemA = Optimizer.vStack.Pop();
            var itemB = Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (!itemA.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        if (!itemB.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        Executex86(4);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.SaveStack(xOp.NextPosition);
        }

        internal static void Executex86(int size)
        {
            new Pop { DestinationReg = Register.EDX };
            new Pop { DestinationReg = Register.EAX };

            switch(size)
            {
                case 0: throw new Exception("Invalid call to Stind");
                case 1:
                    {
                        new Mov
                        {
                            DestinationReg = Register.EAX,
                            DestinationIndirect = true,
                            SourceReg = Register.DL,
                            Size = 8
                        };
                    }
                    break;
                case 2:
                    {
                        new Mov
                        {
                            DestinationReg = Register.EAX,
                            DestinationIndirect = true,
                            SourceReg = Register.DX,
                            Size = 16
                        };
                    }
                    break;
                case 4:
                    {
                        new Mov
                        {
                            DestinationReg = Register.EAX,
                            DestinationIndirect = true,
                            SourceReg = Register.EDX,
                            Size = 32
                        };
                    }
                    break;
                case 8:
                    {
                        // Low := EDX
                        // High := EAX
                        new Pop { DestinationReg = Register.EDI };
                        new Mov { DestinationReg = Register.EDI, DestinationIndirect = true, SourceReg = Register.EDX };
                        new Mov { DestinationReg = Register.EDI, DestinationDisplacement = 4, DestinationIndirect = true, SourceReg = Register.EAX };
                    }
                    break;
                default:
                    throw new Exception("not implemented");
            }
        }
    }
}

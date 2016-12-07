﻿using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Rem_Un)]
    internal class Rem_Un_il : MSIL
    {
        public Rem_Un_il()
            : base(ILCode.Rem_Un)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Rem(v=vs.110).aspx
         * Description : Divides two unsigned values and pushes the remainder onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 2)
                throw new Exception("Internal Compiler Error: vStack.Count < 2");

            var divisor = Optimizer.vStack.Pop();
            var dividend = Optimizer.vStack.Pop();

            var size = Math.Max(Helper.GetTypeSize(divisor.OperandType, Config.TargetPlatform),
                Helper.GetTypeSize(dividend.OperandType, Config.TargetPlatform));

            /* The stack transitional behavior, in sequential order, is:
             * value1 is pushed onto the stack.
             * value2 is pushed onto the stack.
             * value2 and value1 are popped from the stack and the remainder of value1 div value2 computed.
             * The result is pushed onto the stack.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (divisor.IsFloat || dividend.IsFloat || size > 4)
                            throw new Exception(string.Format("UnImplemented '{0}'", msIL));

                        if (!divisor.SystemStack || !dividend.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        new Pop { DestinationReg = Register.EBX };
                        new Pop { DestinationReg = Register.EAX };
                        new Xor { DestinationReg = Register.EDX, SourceReg = Register.EDX };
                        new Div { DestinationReg = Register.EBX };
                        new Push { DestinationReg = Register.EDX };

                        Optimizer.vStack.Push(new StackItem(divisor.OperandType));
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

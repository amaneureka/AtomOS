﻿using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldind_I4)]
    internal class Ldind_I4_il : MSIL
    {
        public Ldind_I4_il()
            : base(ILCode.Ldind_I4)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldind_I4(v=vs.110).aspx
         * Description : Loads a value of type int32 as an int32 onto the evaluation stack indirectly.
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

            new Comment(string.Format("[{0}] : {1}", ToString(), xOp.ToString()));

            Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        Ldind_I_il.Executex86(4, true);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.vStack.Push(new StackItem(typeof(int)));
        }
    }
}

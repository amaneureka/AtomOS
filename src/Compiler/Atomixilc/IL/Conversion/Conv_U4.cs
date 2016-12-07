﻿using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.IL.CodeType;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Conv_U4)]
    internal class Conv_U4_il : MSIL
    {
        public Conv_U4_il()
            : base(ILCode.Conv_U4)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Conv_U4(v=vs.110).aspx
         * Description : Converts the value on top of the evaluation stack to native uint
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 1");

            var item = Optimizer.vStack.Pop();
            var size = Helper.GetTypeSize(item.OperandType, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * value is pushed onto the stack.
             * value is popped from the stack and the conversion operation is attempted.
             * If the conversion is successful, the resulting value is pushed onto the stack.
             */

            new Comment(string.Format("[{0}] : {1}", ToString(), xOp.ToString()));

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (item.IsFloat || size > 4)
                            throw new Exception(string.Format("UnImplemented '{0}'", msIL));

                        if (!item.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        Optimizer.vStack.Push(new StackItem(typeof(uint)));
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

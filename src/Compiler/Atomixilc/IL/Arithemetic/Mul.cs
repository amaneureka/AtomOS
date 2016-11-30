using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Mul)]
    internal class Mul_il : MSIL
    {
        internal Mul_il()
            : base(ILCode.Mul)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Mul(v=vs.110).aspx
         * Description : Multiplies two values and pushes the result on the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Stack<StackItem> vStack)
        {
            if (vStack.Count < 2)
                throw new Exception("Internal Compiler Error: vStack.Count < 2");

            var itemA = vStack.Pop();
            var itemB = vStack.Pop();

            var size = Math.Max(Helper.GetTypeSize(itemA.OperandType, Config.TargetPlatform),
                Helper.GetTypeSize(itemB.OperandType, Config.TargetPlatform));

            /* The stack transitional behavior, in sequential order, is:
             * value1 is pushed onto the stack.
             * value2 is pushed onto the stack.
             * value2 and value1 are popped from the stack; value1 is multiplied by value2.
             * The result is pushed onto the stack.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (itemA.IsFloat || itemA.IsFloat || size > 4)
                            throw new Exception(string.Format("UnImplemented '{0}'", msIL));

                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

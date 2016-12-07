using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Stelem_Ref)]
    internal class Stelem_Ref_il : MSIL
    {
        public Stelem_Ref_il()
            : base(ILCode.Stelem_Ref)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Stelem_Ref(v=vs.110).aspx
         * Description : Replaces the array element at a given index with the object ref value (type O) on the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 3)
                throw new Exception("Internal Compiler Error: vStack.Count < 3");

            /* The stack transitional behavior, in sequential order, is:
             * An object reference to an array, array, is pushed onto the stack.
             * An index value, index, to an element in array is pushed onto the stack.
             * A value of the type specified in the instruction is pushed onto the stack.
             * The value, the index, and the array reference are popped from the stack; the value is put into the array element at the given index.
             */

            new Comment(string.Format("[{0}] : {1}", ToString(), xOp.ToString()));

            Optimizer.vStack.Pop();
            Optimizer.vStack.Pop();
            Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        Stelem_il.Executex86(4);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldarga)]
    internal class Ldarga_il : MSIL
    {
        public Ldarga_il()
            : base(ILCode.Ldarga)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldarga(v=vs.110).aspx
         * Description : Load an argument address onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var index = ((OpVar)xOp).Value;

            int EBPoffset = Ldarg_il.GetArgumentOffset(Config, method, index);

            Type ArgType = null;
            if (!method.IsStatic)
            {
                if (index == 0)
                {
                    ArgType = method.DeclaringType;
                    if (method.DeclaringType.IsValueType)
                        ArgType = ArgType.MakeByRefType();
                }
                else
                    ArgType = method.GetParameters()[index - 1].ParameterType;
            }
            else
                ArgType = method.GetParameters()[index].ParameterType;

            int ArgSize = Helper.GetTypeSize(ArgType, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * The address addr of the argument indexed by index is pushed onto the stack.
             */

            new Comment(string.Format("[{0}] : {1}", ToString(), xOp.ToString()));

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (ArgSize > 4)
                            throw new Exception("Unsupported ArgSize");

                        new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP };
                        new Add { DestinationReg = Register.EAX, SourceRef = "0x" + EBPoffset.ToString("X") };
                        new Push { DestinationReg = Register.EAX };

                        Optimizer.vStack.Push(new StackItem(typeof(uint)));
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.IL.CodeType;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ret)]
    internal class Ret_il : MSIL
    {
        public Ret_il()
            : base(ILCode.Ret)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ret(v=vs.110).aspx
         * Description : Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto the caller's evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var functionInfo = method as MethodInfo;
            var parameters = method.GetParameters();
            var size = parameters.Sum(a => Helper.GetTypeSize(a.ParameterType, Config.TargetPlatform, true));

            int stackCount = 0;
            if (functionInfo != null && functionInfo.ReturnType != typeof(void))
                stackCount = 1;

            if (Optimizer.vStack.Count < stackCount)
                throw new Exception("Internal Compiler Error: vStack.Count < expected size");

            /* The stack transitional behavior, in sequential order, is:
             * The return value is popped from the callee evaluation stack.
             * The return value obtained in step 1 is pushed onto the caller evaluation stack.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (functionInfo != null && Helper.GetTypeSize(functionInfo.ReturnType, Config.TargetPlatform) > 4)
                            throw new Exception(string.Format("UnImplemented '{0}'", msIL));

                        new Xor { DestinationReg = Register.ECX, SourceReg = Register.ECX };

                        if (stackCount > 0)
                        {
                            var item = Optimizer.vStack.Pop();
                            if (!item.SystemStack)
                                throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                            new Pop { DestinationReg = Register.EAX };
                        }

                        new Leave { };
                        new Ret { Offset = (byte)size };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

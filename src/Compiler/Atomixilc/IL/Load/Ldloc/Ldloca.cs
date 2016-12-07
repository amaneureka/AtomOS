using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldloca)]
    internal class Ldloca_il : MSIL
    {
        public Ldloca_il()
            : base(ILCode.Ldloca)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldloca(v=vs.110).aspx
         * Description : Loads the address of the local variable at a specific index onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var index = ((OpVar)xOp).Value;

            var body = method.GetMethodBody();
            var EBPoffset = Helper.GetVariableOffset(body, index, Config.TargetPlatform);

            var varType = body.LocalVariables[index].LocalType;
            var size = Helper.GetTypeSize(varType, Config.TargetPlatform, true);

            /* The stack transitional behavior, in sequential order, is:
             * The address stored in the local variable at the specified index is pushed onto the stack.
             */

            new Comment(string.Format("[{0}] : {1}", ToString(), xOp.ToString()));

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (size > 4)
                            throw new Exception("LocalVariable size > 4 not supported");

                        new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP };
                        new Sub { DestinationReg = Register.EAX, SourceRef = "0x" + (EBPoffset + size).ToString() };
                        new Push { DestinationReg = Register.EAX };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.vStack.Push(new StackItem(typeof(uint)));
        }
    }
}

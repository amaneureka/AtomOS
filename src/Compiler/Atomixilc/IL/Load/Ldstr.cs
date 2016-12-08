using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldstr)]
    internal class Ldstr_il : MSIL
    {
        public Ldstr_il()
            : base(ILCode.Ldstr)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldstr(v=vs.110).aspx
         * Description : Pushes a new object reference to a string literal stored in the metadata.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var str = ((OpString)xOp).Value;

            /* The stack transitional behavior, in sequential order, is:
             * An object reference to a string is pushed onto the stack.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        new Push { DestinationRef = Helper.GetResolvedStringLabel(str) };

                        Optimizer.vStack.Push(new StackItem(typeof(string)));
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

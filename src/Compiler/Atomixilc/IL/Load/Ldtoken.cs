using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldtoken)]
    internal class Ldtoken_il : MSIL
    {
        public Ldtoken_il()
            : base(ILCode.Ldtoken)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldtoken(v=vs.110).aspx
         * Description : Converts a metadata token to its runtime representation, pushing it onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var token = (OpToken)xOp;
            string tokenAddress = null;

            if (token.IsField)
                tokenAddress = token.ValueField.FullName();
            else
                throw new Exception("token.IsType not implemented");

            /* The stack transitional behavior, in sequential order, is:
             * The passed token is converted to a RuntimeHandle and pushed onto the stack.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        new Push { DestinationRef = tokenAddress };

                        Optimizer.vStack.Push(new StackItem(typeof(uint)));
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

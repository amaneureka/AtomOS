using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.IL.CodeType;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Br)]
    internal class Br_il : MSIL
    {
        internal Br_il()
            : base(ILCode.Br)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Br(v=vs.110).aspx
         * Description : Unconditionally transfers control to a target instruction.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 2)
                throw new Exception("Internal Compiler Error: vStack.Count < 2");

            var offset = ((OpBranch)xOp).Value;
            var xTrueLabel = Helper.GetLabel(method, offset);

            /* No evaluation stack behaviors are performed by this operation.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        new Jmp { DestinationRef = xTrueLabel };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

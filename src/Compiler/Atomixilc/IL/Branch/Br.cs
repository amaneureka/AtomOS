/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Br MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
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
        public Br_il()
            : base(ILCode.Br)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Br(v=vs.110).aspx
         * Description : Unconditionally transfers control to a target instruction.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var offset = ((OpBranch)xOp).Value;
            var xTrueLabel = Helper.GetLabel(offset);

            /* No evaluation stack behaviors are performed by this operation.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        new Jmp { DestinationRef = xTrueLabel };

                        Optimizer.SaveStack(offset);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

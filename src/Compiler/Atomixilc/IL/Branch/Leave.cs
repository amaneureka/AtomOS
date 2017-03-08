/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Leave MSIL
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
    [ILImpl(ILCode.Leave)]
    internal class Leave_il : MSIL
    {
        public Leave_il()
            : base(ILCode.Leave)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Leave(v=vs.110).aspx
         * Description : Exits a protected region of code, unconditionally transferring control to a specific target instruction.
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

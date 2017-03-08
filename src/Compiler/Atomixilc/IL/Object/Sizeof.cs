/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Sizeof MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Sizeof)]
    internal class Sizeof_il : MSIL
    {
        public Sizeof_il()
            : base(ILCode.Sizeof)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Sizeof(v=vs.110).aspx
         * Description : Pushes the size, in bytes, of a supplied value type onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var type = ((OpType)xOp).Value;

            /* The stack transitional behavior, in sequential order, is:
             * The size (in bytes) of the supplied value type (valType) is pushed onto the stack.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        new Push { DestinationRef = "0x" + Helper.GetTypeSize(type, Config.TargetPlatform).ToString("X") };
                        Optimizer.vStack.Push(new StackItem(typeof(uint)));
                        Optimizer.SaveStack(xOp.NextPosition);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Ldc_R8 MSIL
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
    [ILImpl(ILCode.Ldc_R8)]
    internal class Ldc_R8_il : MSIL
    {
        public Ldc_R8_il()
            : base(ILCode.Ldc_R8)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldc_R8(v=vs.110).aspx
         * Description : Pushes a supplied value of type float64 onto the evaluation stack as type F (float).
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var num = ((OpDouble)xOp).Value;
            var xData = BitConverter.GetBytes(num);

            /* The stack transitional behavior, in sequential order, is:
             * The value num is pushed onto the stack
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {

                        new Push { DestinationRef = "0x" + BitConverter.ToUInt32(xData, 4).ToString("X") };
                        new Push { DestinationRef = "0x" + BitConverter.ToUInt32(xData, 0).ToString("X") };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.vStack.Push(new StackItem(typeof(double)));
        }
    }
}

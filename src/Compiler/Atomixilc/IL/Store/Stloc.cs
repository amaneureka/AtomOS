/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stloc MSIL
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
    [ILImpl(ILCode.Stloc)]
    internal class Stloc_il : MSIL
    {
        public Stloc_il()
            : base(ILCode.Stloc)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Stloc(v=vs.110).aspx
         * Description : Pops the current value from the top of the evaluation stack and stores it in a the local variable list at a specified index.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 1");

            var index = ((OpVar)xOp).Value;

            var body = method.GetMethodBody();
            var EBPoffset = Helper.GetVariableOffset(body, index, Config.TargetPlatform);

            var varType = body.LocalVariables[index].LocalType;
            var size = Helper.GetTypeSize(varType, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * A value is popped off of the stack and placed in local variable index.
             */

            var item = Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (size > 4)
                            throw new Exception("LocalVariable size > 4 not supported");

                        if (!item.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        new Pop { DestinationReg = Register.EAX };
                        new Mov
                        {
                            DestinationReg = Register.EBP,
                            DestinationIndirect = true,
                            DestinationDisplacement = -EBPoffset,
                            SourceReg = Register.EAX
                        };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.SaveStack(xOp.NextPosition);
        }
    }
}

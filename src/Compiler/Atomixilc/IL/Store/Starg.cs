/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Starg MSIL
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
    [ILImpl(ILCode.Starg)]
    internal class Starg_il : MSIL
    {
        public Starg_il()
            : base(ILCode.Starg)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Starg(v=vs.110).aspx
         * Description : Stores the value on top of the evaluation stack in the argument slot at a specified index.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 1");

            var index = ((OpVar)xOp).Value;

            int EBPoffset = Ldarg_il.GetArgumentOffset(Config, method, index);

            Type ArgType = null;
            if (!method.IsStatic)
            {
                if (index == 0)
                {
                    ArgType = method.DeclaringType;
                    if (method.DeclaringType.IsValueType)
                        ArgType = ArgType.MakeByRefType();
                }
                else
                    ArgType = method.GetParameters()[index - 1].ParameterType;
            }
            else
                ArgType = method.GetParameters()[index].ParameterType;

            int ArgSize = Helper.GetTypeSize(ArgType, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * The value currently on top of the stack is popped and placed in argument slot num.
             */

            var item = Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (ArgSize > 4)
                            throw new Exception("Unsupported ArgSize");

                        if (!item.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        new Pop { DestinationReg = Register.EAX };
                        new Mov
                        {
                            DestinationReg = Register.EBP,
                            DestinationIndirect = true,
                            DestinationDisplacement = EBPoffset,
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

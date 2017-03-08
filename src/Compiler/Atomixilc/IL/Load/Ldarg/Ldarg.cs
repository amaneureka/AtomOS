/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldarg MSIL
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
    [ILImpl(ILCode.Ldarg)]
    internal class Ldarg_il : MSIL
    {
        public Ldarg_il()
            : base(ILCode.Ldarg)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldarg(v=vs.110).aspx
         * Description : Loads an argument (referenced by a specified index value) onto the stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var index = ((OpVar)xOp).Value;

            int EBPoffset = GetArgumentOffset(Config, method, index);

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
             * The argument value at index is pushed onto the stack.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (ArgSize > 4)
                            throw new Exception("Unsupported ArgSize");

                        new Push
                        {
                            DestinationReg = Register.EBP,
                            DestinationDisplacement = EBPoffset,
                            DestinationIndirect = true
                        };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.vStack.Push(new StackItem(ArgType));
            Optimizer.SaveStack(xOp.NextPosition);
        }

        internal static int GetArgumentOffset(Options Config, MethodBase method, int paramIndex)
        {
            if (Config.TargetPlatform != Architecture.x86)
                throw new Exception("Unsupported Platform");

            var xparams = method.GetParameters();

            if (!method.IsStatic) paramIndex--;

            int offset = 8, index = xparams.Length - 1;
            while(index > paramIndex)
            {
                offset += Helper.GetTypeSize(xparams[index].ParameterType, Config.TargetPlatform, true);
                index--;
            }

            return offset;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Div_Un)]
    internal class Div_Un_il : MSIL
    {
        internal Div_Un_il()
            : base(ILCode.Div)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Div_Un(v=vs.110).aspx
         * Description : Divides two unsigned integer values and pushes the result (int32) onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Stack<StackItem> vStack)
        {
            if (vStack.Count < 2)
                throw new Exception("Internal Compiler Error: vStack.Count < 2");

            var divisor = vStack.Pop();
            var dividend = vStack.Pop();

            var size = Math.Max(Helper.GetTypeSize(divisor.OperandType, Config.TargetPlatform),
                Helper.GetTypeSize(dividend.OperandType, Config.TargetPlatform));

            /* The stack transitional behavior, in sequential order, is:
             * value1 is pushed onto the stack.
             * value2 is pushed onto the stack.
             * value2 and value1 are popped from the stack; value1 is divided by value2.
             * The result is pushed onto the stack.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (divisor.IsFloat || dividend.IsFloat || size > 4)
                            throw new Exception(string.Format("UnImplemented '{0}'", msIL));

                        if (!dividend.RegisterRef.HasValue || dividend.RegisterRef != Register.EAX || dividend.IsIndirect)
                        {
                            new Mov
                            {
                                DestinationReg = Register.EAX,
                                SourceReg = dividend.RegisterRef,
                                SourceRef = dividend.AddressRef,
                                SourceIndirect = dividend.IsIndirect,
                                SourceDisplacement = dividend.Displacement
                            };
                        }

                        new Xor { DestinationReg = Register.EDX, SourceReg = Register.EDX };
                        new Div
                        {
                            DestinationReg = divisor.RegisterRef,
                            DestinationRef = divisor.AddressRef,
                            DestinationDisplacement = divisor.Displacement,
                            DestinationIndirect = divisor.IsIndirect
                        };

                        vStack.Push(new StackItem { RegisterRef = Register.EAX, OperandType = typeof(UInt32) });
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

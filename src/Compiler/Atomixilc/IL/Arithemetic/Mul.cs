using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Mul)]
    internal class Mul_il : MSIL
    {
        internal Mul_il()
            : base(ILCode.Mul)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Mul(v=vs.110).aspx
         * Description : Multiplies two values and pushes the result on the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 2)
                throw new Exception("Internal Compiler Error: vStack.Count < 2");

            var itemA = Optimizer.vStack.Pop();
            var itemB = Optimizer.vStack.Pop();

            var size = Math.Max(Helper.GetTypeSize(itemA.OperandType, Config.TargetPlatform),
                Helper.GetTypeSize(itemB.OperandType, Config.TargetPlatform));

            /* The stack transitional behavior, in sequential order, is:
             * value1 is pushed onto the stack.
             * value2 is pushed onto the stack.
             * value2 and value1 are popped from the stack; value1 is multiplied by value2.
             * The result is pushed onto the stack.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (itemA.IsFloat || itemA.IsFloat || size > 4)
                            throw new Exception(string.Format("UnImplemented '{0}'", msIL));

                        if (itemA.RegisterRef.HasValue)
                            Optimizer.FreeRegister(itemA.RegisterRef.Value);

                        if (itemB.RegisterRef.HasValue)
                            Optimizer.FreeRegister(itemB.RegisterRef.Value);

                        if (itemB.SystemStack)
                        {
                            new Pop { DestinationReg = Register.ESI };
                        }

                        if (itemA.SystemStack)
                        {
                            new Pop { DestinationReg = Register.EAX };
                        }
                        else
                        {
                            new Mov
                            {
                                DestinationReg = Register.EAX,
                                SourceReg = itemA.RegisterRef,
                                SourceIndirect = itemA.IsIndirect,
                                SourceDisplacement = itemA.Displacement,
                                SourceRef = itemA.AddressRef
                            };
                        }

                        if (itemB.SystemStack)
                        {
                            new IMul { DestinationReg = Register.ESI };
                        }
                        else
                        {
                            new IMul
                            {
                                DestinationReg = itemA.RegisterRef,
                                DestinationIndirect = itemA.IsIndirect,
                                DestinationDisplacement = itemA.Displacement,
                                DestinationRef = itemA.AddressRef
                            };
                        }

                        Register? NonVolatileRegister = null;
                        Optimizer.GetNonVolatileRegister(ref NonVolatileRegister);

                        if (NonVolatileRegister.HasValue)
                        {
                            new Mov
                            {
                                DestinationReg = NonVolatileRegister.Value,
                                SourceReg = Register.EAX
                            };
                            Optimizer.AllocateRegister(NonVolatileRegister.Value);
                            Optimizer.vStack.Push(new StackItem(NonVolatileRegister.Value, typeof(Int32)));
                        }
                        else
                        {
                            new Push { DestinationReg = Register.EAX };
                            Optimizer.vStack.Push(new StackItem(typeof(Int32)));
                        }
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

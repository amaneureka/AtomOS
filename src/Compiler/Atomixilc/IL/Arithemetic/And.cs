using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.And)]
    internal class And_il : MSIL
    {
        internal And_il()
            : base(ILCode.And)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.And(v=vs.110).aspx
         * Description : Computes the bitwise AND of two values and pushes the result onto the evaluation stack.
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
             * value1 and value2 are popped from the stack; the bitwise AND of the two values is computed.
             * The result is pushed onto the stack.
             */

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (itemA.IsFloat || itemB.IsFloat || size > 4)
                            throw new Exception(string.Format("UnImplemented '{0}'", msIL));

                        if (itemA.RegisterRef.HasValue)
                            Optimizer.FreeRegister(itemA.RegisterRef.Value);

                        if (itemB.RegisterRef.HasValue)
                            Optimizer.FreeRegister(itemB.RegisterRef.Value);

                        if (itemA.RegisterOnly)
                        {
                            if (itemB.RegisterOnly)
                            {
                                new And { DestinationReg = itemA.RegisterRef, SourceReg = itemB.RegisterRef };
                            }
                            else if (itemB.SystemStack)
                            {
                                new Pop { DestinationReg = Register.EAX };
                                new And { DestinationReg = itemA.RegisterRef, SourceReg = Register.EAX };
                            }
                            else
                            {
                                new And
                                {
                                    DestinationReg = itemA.RegisterRef,
                                    SourceReg = itemB.RegisterRef,
                                    SourceIndirect = itemB.IsIndirect,
                                    SourceDisplacement = itemB.Displacement,
                                    SourceRef = itemB.AddressRef
                                };
                            }

                            Optimizer.AllocateRegister(itemA.RegisterRef.Value);
                            Optimizer.vStack.Push(new StackItem(itemA.RegisterRef.Value, itemA.OperandType));
                        }
                        else if (itemA.SystemStack)
                        {
                            new Pop { DestinationReg = Register.EAX };
                            if (itemB.RegisterOnly)
                            {
                                new And { DestinationReg = itemB.RegisterRef, SourceReg = Register.EAX };
                                Optimizer.AllocateRegister(itemB.RegisterRef.Value);
                                Optimizer.vStack.Push(new StackItem(itemB.RegisterRef.Value, itemA.OperandType));
                            }
                            else if (itemB.SystemStack)
                            {
                                Register? NonVolatileRegister = null;
                                Optimizer.GetNonVolatileRegister(ref NonVolatileRegister);

                                if (NonVolatileRegister.HasValue)
                                {
                                    new Pop { DestinationReg = NonVolatileRegister.Value };
                                    new And { DestinationReg = NonVolatileRegister.Value, SourceReg = Register.EAX };
                                    Optimizer.AllocateRegister(NonVolatileRegister.Value);
                                    Optimizer.vStack.Push(new StackItem(NonVolatileRegister.Value, itemA.OperandType));
                                }
                                else
                                {
                                    new And { DestinationReg = Register.ESP, DestinationIndirect = true, SourceReg = Register.EAX };
                                    Optimizer.vStack.Push(new StackItem(itemA.OperandType));
                                }
                            }
                            else
                            {
                                Register? NonVolatileRegister = null;
                                Optimizer.GetNonVolatileRegister(ref NonVolatileRegister);

                                if (NonVolatileRegister.HasValue)
                                {
                                    new Mov
                                    {
                                        DestinationReg = NonVolatileRegister.Value,
                                        SourceReg = itemB.RegisterRef,
                                        SourceIndirect = itemB.IsIndirect,
                                        SourceDisplacement = itemB.Displacement,
                                        SourceRef = itemB.AddressRef
                                    };
                                    new And { DestinationReg = NonVolatileRegister.Value, SourceReg = Register.EAX };
                                    Optimizer.AllocateRegister(NonVolatileRegister.Value);
                                    Optimizer.vStack.Push(new StackItem(NonVolatileRegister.Value, itemA.OperandType));
                                }
                                else
                                {
                                    new And
                                    {
                                        DestinationReg = Register.EAX,
                                        SourceReg = itemB.RegisterRef,
                                        SourceIndirect = itemB.IsIndirect,
                                        SourceDisplacement = itemB.Displacement,
                                        SourceRef = itemB.AddressRef
                                    };
                                    new Push { DestinationReg = Register.EAX };
                                    Optimizer.vStack.Push(new StackItem(itemA.OperandType));
                                }
                            }
                        }
                        else
                        {
                            if (itemB.RegisterOnly)
                            {
                                new And
                                {
                                    DestinationReg = itemB.RegisterRef,
                                    SourceReg = itemA.RegisterRef,
                                    SourceIndirect = itemA.IsIndirect,
                                    SourceDisplacement = itemA.Displacement,
                                    SourceRef = itemA.AddressRef
                                };
                                Optimizer.AllocateRegister(itemB.RegisterRef.Value);
                                Optimizer.vStack.Push(new StackItem(itemB.RegisterRef.Value, itemA.OperandType));
                            }
                            else if (itemB.SystemStack)
                            {
                                Register? NonVolatileRegister = null;
                                Optimizer.GetNonVolatileRegister(ref NonVolatileRegister);

                                if (!NonVolatileRegister.HasValue)
                                    NonVolatileRegister = Register.EAX;

                                new Pop { DestinationReg = NonVolatileRegister.Value };
                                new And
                                {
                                    DestinationReg = NonVolatileRegister.Value,
                                    SourceReg = itemA.RegisterRef,
                                    SourceIndirect = itemA.IsIndirect,
                                    SourceDisplacement = itemA.Displacement,
                                    SourceRef = itemA.AddressRef
                                };

                                if (NonVolatileRegister != Register.EAX)
                                {
                                    Optimizer.AllocateRegister(NonVolatileRegister.Value);
                                    Optimizer.vStack.Push(new StackItem(NonVolatileRegister.Value, itemA.OperandType));
                                }
                                else
                                {
                                    new Push { DestinationReg = Register.EAX };
                                    Optimizer.vStack.Push(new StackItem(itemA.OperandType));
                                }
                            }
                            else
                            {
                                Register? NonVolatileRegister = null;
                                Optimizer.GetNonVolatileRegister(ref NonVolatileRegister);

                                if (!NonVolatileRegister.HasValue)
                                    NonVolatileRegister = Register.EAX;

                                new Mov
                                {
                                    DestinationReg = NonVolatileRegister.Value,
                                    SourceReg = itemA.RegisterRef,
                                    SourceIndirect = itemA.IsIndirect,
                                    SourceDisplacement = itemA.Displacement,
                                    SourceRef = itemA.AddressRef
                                };

                                new And
                                {
                                    DestinationReg = NonVolatileRegister.Value,
                                    SourceReg = itemB.RegisterRef,
                                    SourceIndirect = itemB.IsIndirect,
                                    SourceDisplacement = itemB.Displacement,
                                    SourceRef = itemB.AddressRef
                                };

                                if (NonVolatileRegister != Register.EAX)
                                {
                                    Optimizer.AllocateRegister(NonVolatileRegister.Value);
                                    Optimizer.vStack.Push(new StackItem(NonVolatileRegister.Value, itemA.OperandType));
                                }
                                else
                                {
                                    new Push { DestinationReg = Register.EAX };
                                    Optimizer.vStack.Push(new StackItem(itemA.OperandType));
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

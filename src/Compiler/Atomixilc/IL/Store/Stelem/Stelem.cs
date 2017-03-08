/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Stelem MSIL
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
    [ILImpl(ILCode.Stelem)]
    internal class Stelem_il : MSIL
    {
        public Stelem_il()
            : base(ILCode.Stelem)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Stelem(v=vs.110).aspx
         * Description : Replaces the array element at a given index with the value on the evaluation stack, whose type is specified in the instruction.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 3)
                throw new Exception("Internal Compiler Error: vStack.Count < 3");

            var operand = ((OpType)xOp).Value;
            var size = Helper.GetTypeSize(operand, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * An object reference to an array, array, is pushed onto the stack.
             * An index value, index, to an element in array is pushed onto the stack.
             * A value of the type specified in the instruction is pushed onto the stack.
             * The value, the index, and the array reference are popped from the stack; the value is put into the array element at the given index.
             */

            var itemA = Optimizer.vStack.Pop();
            var itemB = Optimizer.vStack.Pop();
            var itemC = Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (size > 4)
                            throw new Exception("LocalVariable size > 4 not supported");

                        if (!itemA.SystemStack || !itemB.SystemStack || !itemC.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        Executex86(size);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.SaveStack(xOp.NextPosition);
        }

        internal static void Executex86(int size)
        {
            new Pop { DestinationReg = Register.EDI };
            new Pop { DestinationReg = Register.EAX };

            switch (size)
            {
                case 1: break;
                case 2:
                    new Add { DestinationReg = Register.EAX, SourceReg = Register.EAX };
                    break;
                case 4:
                    new Shl { DestinationReg = Register.EAX, SourceRef = "0x2" };
                    break;
                default:
                    throw new Exception("size not supported");
            }

            new Pop { DestinationReg = Register.EDX };
            new Add { DestinationReg = Register.EAX, SourceReg = Register.EDX };
            new Mov { DestinationReg = Register.EDX, SourceReg = Register.EDI };

            switch (size)
            {
                case 0: break;
                case 1:
                    {
                        new Mov
                        {
                            DestinationReg = Register.EAX,
                            DestinationDisplacement = 0x10,
                            DestinationIndirect = true,
                            SourceReg = Register.DL,
                            Size = 8
                        };
                    }
                    break;
                case 2:
                    {
                        new Mov
                        {
                            DestinationReg = Register.EAX,
                            DestinationDisplacement = 0x10,
                            DestinationIndirect = true,
                            SourceReg = Register.DX,
                            Size = 16
                        };
                    }
                    break;
                case 4:
                    {
                        new Mov
                        {
                            DestinationReg = Register.EAX,
                            DestinationDisplacement = 0x10,
                            DestinationIndirect = true,
                            SourceReg = Register.EDX,
                            Size = 32
                        };
                    }
                    break;
                default:
                    throw new Exception("not implemented");
            }
        }
    }
}

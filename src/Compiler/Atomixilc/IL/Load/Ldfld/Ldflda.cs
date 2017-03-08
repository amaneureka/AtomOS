/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldflda MSIL
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
    [ILImpl(ILCode.Ldflda)]
    internal class Ldflda_il : MSIL
    {
        public Ldflda_il()
            : base(ILCode.Ldflda)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldflda(v=vs.110).aspx
         * Description : Finds the address of a field in the object whose reference is currently on the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 1");

            var field = ((OpField)xOp).Value;
            var offset = Helper.GetFieldOffset(field.DeclaringType, field, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * An object reference (or pointer) is pushed onto the stack.
             * The object reference (or pointer) is popped from the stack; the address of the specified field in the object is found.
             * The address of the specified field is pushed onto the stack.
             */

            var item = Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (!item.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        new Add { DestinationReg = Register.ESP, DestinationIndirect = true, SourceRef = "0x" + offset.ToString("X") };
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }

            Optimizer.vStack.Push(new StackItem(typeof(uint)));
            Optimizer.SaveStack(xOp.NextPosition);
        }
    }
}

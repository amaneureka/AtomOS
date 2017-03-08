/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Switch MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.IL.CodeType;
using Atomixilc.Machine.x86;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Switch)]
    internal class Switch_il : MSIL
    {
        public Switch_il()
            : base(ILCode.Switch)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Switch(v=vs.110).aspx
         * Description : Implements a jump table.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 1)
                throw new Exception("Internal Compiler Error: vStack.Count < 1");

            var branches = ((OpSwitch)xOp).Value;

            /* The stack transitional behavior, in sequential order, is:
             * A value is pushed onto the stack.
             * The value is popped off the stack and execution is transferred to the instruction at the offset indexed by the value, where the value is less than N.
             */

            var item = Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (!item.SystemStack)
                            throw new Exception(string.Format("UnImplemented-RegisterType '{0}'", msIL));

                        new Pop { DestinationReg = Register.EAX };
                        for (int i = 0; i < branches.Length; i++)
                        {
                            new Cmp { DestinationReg = Register.EAX, SourceRef = "0x" + i.ToString("X") };
                            new Jmp { Condition = ConditionalJump.JE, DestinationRef = Helper.GetLabel(branches[i]) };
                            Optimizer.SaveStack(branches[i]);
                        }

                        Optimizer.SaveStack(xOp.NextPosition);
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Stfld)]
    internal class Stfld_il : MSIL
    {
        public Stfld_il()
            : base(ILCode.Stfld)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Stfld(v=vs.110).aspx
         * Description : Replaces the value stored in the field of an object reference or pointer with a new value.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            if (Optimizer.vStack.Count < 2)
                throw new Exception("Internal Compiler Error: vStack.Count < 2");

            var field = ((OpField)xOp).Value;
            var offset = Helper.GetFieldOffset(field.DeclaringType, field, Config.TargetPlatform);
            var size = Helper.GetTypeSize(field.FieldType, Config.TargetPlatform);

            /* The stack transitional behavior, in sequential order, is:
             * An object reference or pointer is pushed onto the stack.
             * A value is pushed onto the stack.
             * The value and the object reference/pointer are popped from the stack; the value of field in the object is replaced with the supplied value.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            Optimizer.vStack.Pop();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (size > 4)
                            throw new Exception("Field Size > 4 not supported");

                        new Pop { DestinationReg = Register.EAX };
                        new Pop { DestinationReg = Register.EDX };
                        new Add { DestinationReg = Register.EDX, SourceRef = "0x" + offset.ToString("X") };

                        switch(size)
                        {
                            case 1:
                            case 2:
                            case 4:
                                {
                                    new Mov { DestinationReg = Register.EDX, DestinationIndirect = true, SourceReg = Register.EAX, Size = (byte)(size * 8) };
                                }
                                break;
                            case 3:
                                {
                                    new Mov { DestinationReg = Register.EDX, DestinationIndirect = true, SourceReg = Register.EAX, Size = 16 };
                                    new Shr { DestinationReg = Register.EAX, SourceRef = "0x10" };
                                    new Mov { DestinationReg = Register.EDX, DestinationIndirect = true, DestinationDisplacement = 2, SourceReg = Register.EAX, Size = 8 };
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc.IL
{
    [ILImpl(ILCode.Ldsflda)]
    internal class Ldsflda_il : MSIL
    {
        public Ldsflda_il()
            : base(ILCode.Ldsflda)
        {

        }

        /*
         * URL : https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.Ldsflda(v=vs.110).aspx
         * Description : Pushes the address of a static field onto the evaluation stack.
         */
        internal override void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer)
        {
            var field = ((OpField)xOp).Value;
            var fieldName = field.FullName();
            var size = Helper.GetTypeSize(field.FieldType, Config.TargetPlatform);

            string cctor_addref = null;

            var cctor = (field.DeclaringType.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) ?? new ConstructorInfo[0]).SingleOrDefault();
            if (cctor != null)
                cctor_addref = cctor.FullName();

            /* The stack transitional behavior, in sequential order, is:
             * The value of the specific field is pushed onto the stack.
             */

            new Comment(string.Format("[{0}] : {1} => {2}", ToString(), xOp.ToString(), Optimizer.vStack.Count));

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    {
                        if (!string.IsNullOrEmpty(cctor_addref) && cctor != method)
                            new Call { DestinationRef = cctor_addref };

                        new Push { DestinationRef = fieldName };

                        Optimizer.vStack.Push(new StackItem(typeof(uint)));
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported target platform '{0}' for MSIL '{1}'", Config.TargetPlatform, msIL));
            }
        }
    }
}

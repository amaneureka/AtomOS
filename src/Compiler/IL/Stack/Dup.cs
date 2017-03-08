/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Dup MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Dup)]
    public class Dup : MSIL
    {
        public Dup(Compiler Cmp)
            : base("dup", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //We just want the size so we peek
            var xStack = Core.vStack.Peek();
            var xStackCount = (int)(xStack.Size.Align() / 4);//Calculate Stack Size

            /*
                value is pushed onto the stack.
                value is popped off of the stack for duplication.
                value is pushed back onto the stack.
                A duplicate value is pushed onto the stack.
            */

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        //***What we are going to do is***
                        //1) Just Push the memory at ESP with size as given above
                        for (int i = xStackCount; i > 0; i--)
                        {
                            Core.AssemblerCode.Add(
                                new Assembler.x86.Push {
                                    DestinationReg = Registers.ESP,
                                    DestinationIndirect = true,
                                    DestinationDisplacement = (int)((xStackCount - 1) * 4) });
                        }
                    }
                    break;
                #endregion
                #region _x64_
                case CPUArch.x64:
                    {

                    }
                    break;
                #endregion
                #region _ARM_
                case CPUArch.ARM:
                    {

                    }
                    break;
                #endregion
            }

            //Push the duplicate one
            Core.vStack.Push(xStack.Size, xStack.Type);
        }
    }
}

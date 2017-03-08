/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldlen MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Ldlen)]
    public class Ldlen : MSIL
    {
        public Ldlen(Compiler Cmp)
            : base("ldlen", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            /*
                An object reference to an array is pushed onto the stack.
                The array reference is popped from the stack and the length is computed.
                The length is pushed onto the stack.
            */

            //Pop Array from stack
            Core.vStack.Pop();
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        //***What we are going to do is***
                        //1) pop array address into EAX
                        //2) Pop 32 bit memory at EAX + 0x8 to stack, 0x8 --> Length offset (Newarr)
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX, DestinationDisplacement = 0x8, DestinationIndirect = true });
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
            //Push length onto stack
            Core.vStack.Push(4, typeof(uint));
        }
    }
}

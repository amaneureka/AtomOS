/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ldelema MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Ldelema)]
    public class Ldelema : MSIL
    {
        public Ldelema(Compiler Cmp)
            : base("ldelema", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xOperand = ((OpType)instr).Value;
            var xSize = xOperand.SizeOf();

            Core.vStack.Pop();//Array
            Core.vStack.Pop();//Element index

            /*
                An object reference array is pushed onto the stack.
                An index value index is pushed onto the stack.
                index and array are popped from the stack;
                the address stored at position index in array is looked up.
                The address is pushed onto the stack.
            */

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        //***What we are going to do is***
                        //1) Pop Element index to EAX
                        //2) Set content of EDX = 16
                        //3) Do EDX:EAX = EDX * EAX
                        //4) Add EAX by 16 because 0x10 is data offset
                        //5) Pop Array pointer into EDX
                        //6) Add The Array pointer to EAX content to get element offset
                        //7) Push the element offset to stack
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });//element index
                        Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceRef = "0x" + xSize.ToString("X") });//Size of each element of array
                        Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EDX });//Multiply to get offset of element
                        Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceRef = "0x10" });//As the first 12 are header and other 4 is size so data offset starts at 16
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EDX });//pop pointer to array
                        Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EDX, SourceReg = Registers.EAX });
                        Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EDX });
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

            Core.vStack.Push(4, typeof(uint));
        }
    }
}

/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          call x86 instruction
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.IO;

namespace Atomix.Assembler.x86
{
    public class Call : Instruction
    {
        public readonly string Address;
        public readonly bool FunctionLabel;

        public Call(string aAddress, bool aFunctionLabel = false)
            :base ("call")
        {
            Address = aAddress;
            // If FunctionLabel is set then it will look into Label Dictionary for real symbol name
            // check Compiler.FlushAsmFile();
            FunctionLabel = aFunctionLabel;
        }

        public override void FlushText(StreamWriter aSW)
        {
            aSW.WriteLine("call " + Address);
        }

        public static void FlushText(StreamWriter aSW, string aAddress)
        {
            aSW.WriteLine("call " + aAddress);
        }
    }
}

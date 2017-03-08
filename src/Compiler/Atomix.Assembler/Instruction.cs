/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Instruction class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.IO;
using Atomix.CompilerExt;

namespace Atomix.Assembler
{
    public abstract class Instruction
    {
        public readonly string Code;
        public readonly CPUArch CPUArch;

        public Instruction(string aMnemonic, CPUArch aCPUArch = CPUArch.x86)
        {
            Code = aMnemonic;
            CPUArch = aCPUArch;
        }

        public virtual void FlushText(StreamWriter aSW)
        {
            aSW.WriteLine(Code);
        }
    }
}

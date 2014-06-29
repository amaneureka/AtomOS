using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Atomix.CompilerExt;

namespace Atomix.Assembler
{
    public abstract class Instruction
    {
        public readonly string Code;
        public CPUArch CPUArch;

        public Instruction(string aMnemonic, CPUArch aCPUArch = CPUArch.x86)
        {
            this.Code = aMnemonic;
            this.CPUArch = aCPUArch;
        }

        public virtual void FlushText(StreamWriter sw)
        {
            sw.WriteLine(Code);
        }
    }
}

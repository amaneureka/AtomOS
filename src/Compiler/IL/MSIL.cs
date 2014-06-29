using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Atomix;
using Atomix.Assembler;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    //http://en.wikipedia.org/wiki/List_of_CIL_instructions
    public abstract class MSIL
    {
        protected Compiler Compiler;
        protected CPUArch CPUArch;
        protected string IL;

        public MSIL(string aMnemonic, Compiler Cmp)
        {
            this.IL = aMnemonic;
            this.Compiler = Cmp;
            this.CPUArch = ILCompiler.CPUArchitecture;
            ILCompiler.Logger.Write(aMnemonic + "...Loaded");
        }
        public virtual void Execute(ILOpCode instr, MethodBase aMethod) 
        {
            Core.AssemblerCode.Add(new Literal(IL));
        }
    }
}

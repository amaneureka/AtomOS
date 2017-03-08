/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          MSIL abstract class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

using Atomix.Assembler;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    public abstract class MSIL
    {
        protected Compiler Compiler;
        protected CPUArch CPUArch;
        protected string IL;

        public MSIL(string aMnemonic, Compiler aCompiler)
        {
            IL = aMnemonic;
            Compiler = aCompiler;
            CPUArch = ILCompiler.CPUArchitecture;
            ILCompiler.Logger.Write(aMnemonic + "...Loaded");
        }

        public virtual void Execute(ILOpCode instr, MethodBase aMethod)
        {
            Core.AssemblerCode.Add(new Literal(IL));
        }
    }
}

/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          OnlySize type Instructions abstract class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.IO;

namespace Atomix.Assembler.x86
{
    public abstract class OnlySize : Instruction
    {
        public byte Address;

        public OnlySize(string aMnemonic)
            : base(aMnemonic)  { }

        public override void FlushText(StreamWriter aSW)
        {
            aSW.WriteLine(string.Format("{0} 0x{1}", Code, Address.ToString("X")));
        }
    }
}

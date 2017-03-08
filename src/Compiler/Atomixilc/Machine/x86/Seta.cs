/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          x86 Seta Instruction
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Machine.x86
{
    public class Seta : Instruction
    {
        public Register DestinationReg;

        public Seta()
            : base("seta")
        {

        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Mnemonic, DestinationReg);
        }
    }
}

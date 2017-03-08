/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Architecture Instructions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Machine
{
    public abstract class Instruction
    {
        internal static FunctionalBlock Block;

        public readonly string Mnemonic;

        public Instruction(string aMnemonic)
        {
            Mnemonic = aMnemonic;
            if (Block == null)
            {
                Verbose.Warning("Floating Instructions");
                return;
            }
            Block.Body.Add(this);
        }

        public override string ToString()
        {
            return Mnemonic.ToString();
        }
    }
}

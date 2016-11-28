using System;

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

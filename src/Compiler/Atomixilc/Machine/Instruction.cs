/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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

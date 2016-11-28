using System;

namespace Atomixilc.Machine.x86
{
    public class OnlySize : Instruction
    {
        public byte? Offset;

        public OnlySize(string aMnemonic)
            :base(aMnemonic)
        {

        }

        public override string ToString()
        {
            if (!Offset.HasValue)
            {
                Offset = 0;
                Verbose.Warning("{0} : Offset not set", Mnemonic);
            }
            return string.Format("{0} 0x{1}", Mnemonic, Offset.Value.ToString("X"));
        }
    }
}

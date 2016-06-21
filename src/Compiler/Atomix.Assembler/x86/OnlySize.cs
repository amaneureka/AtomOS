/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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

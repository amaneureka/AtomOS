/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Jmp x86 instruction
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Assembler.x86
{
    public class Jmp : Instruction
    {
        public ConditionalJumpEnum? Condition;
        public string DestinationRef;

        public Jmp()
            : base("jmp")
        { }

        public override void FlushText(System.IO.StreamWriter aSW)
        {
            if (!Condition.HasValue)
                Condition = ConditionalJumpEnum.JMP;
            aSW.WriteLine(string.Format("{0} near {1}", Condition.ToString(), DestinationRef));
        }        
    }
}

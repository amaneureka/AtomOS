﻿/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Jmp x86 instruction
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.IO;

namespace Atomix.Assembler.x86
{
    public class Jmp : Instruction
    {
        public ConditionalJumpEnum? Condition;
        public string DestinationRef;

        public Jmp()
            : base("jmp")
        { }

        public override void FlushText(StreamWriter aSW)
        {
            if (!Condition.HasValue)
                Condition = ConditionalJumpEnum.JMP;

            if (DestinationRef.StartsWith("."))
                DestinationRef = Label.PrimaryLabel + DestinationRef;

            var jmpStr = Condition.ToString().ToLower();
            if (Condition == ConditionalJumpEnum.JMP)
                aSW.WriteLine(string.Format("{0} {1}", jmpStr, DestinationRef));
            else
                aSW.WriteLine(string.Format("{0} near {1}", jmpStr, DestinationRef));
        }
    }
}

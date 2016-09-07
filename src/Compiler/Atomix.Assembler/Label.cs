/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Label class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.IO;

namespace Atomix.Assembler
{
    public class Label : Instruction
    {
        public static string PrimaryLabel;
        public readonly string Name;
        public readonly string FinalisedName;

        public Label(string aName)
            :base ("Label")
        {
            Name = aName;
            if (aName.StartsWith("."))
            {
                FinalisedName = PrimaryLabel + Name;
            }
            else
            {
                FinalisedName = aName;
                PrimaryLabel = FinalisedName;
            }
        }

        public override void FlushText(StreamWriter aSW)
        {
            aSW.WriteLine(FinalisedName + ":");
        }
    }
}

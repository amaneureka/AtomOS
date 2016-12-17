/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Assembly Labels
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Machine
{
    public class Label : Instruction
    {
        string mlabel;

        public static string Primary;

        public Label(string aLabel)
            :base(string.Empty)
        {
            if (aLabel.StartsWith("."))
                mlabel = Primary + aLabel;
            else
            {
                Primary = aLabel;
                mlabel = aLabel;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}:", mlabel);
        }
    }
}

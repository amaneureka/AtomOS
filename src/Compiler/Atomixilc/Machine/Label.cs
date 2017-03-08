/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
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

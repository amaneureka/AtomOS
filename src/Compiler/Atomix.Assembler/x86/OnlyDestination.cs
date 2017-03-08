/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          OnlyDestination type Instructions abstract class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.IO;

namespace Atomix.Assembler.x86
{
    public abstract class OnlyDestination : Instruction
    {
        public Registers? DestinationReg;
        public string DestinationRef;
        public bool DestinationIndirect;
        public int? DestinationDisplacement;
        public byte Size;

        public OnlyDestination(string aMnemonic)
            : base(aMnemonic)  { }

        public override void FlushText(StreamWriter aSW)
        {
            string des = DestinationReg.HasValue ? DestinationReg.ToString() : DestinationRef;

            if (DestinationDisplacement > 0)
                des = des + " + 0x" + ((uint)DestinationDisplacement).ToString("X");
            else if (DestinationDisplacement < 0)
                des = des + " - 0x" + ((int)(-1 * DestinationDisplacement)).ToString("X");

            if (DestinationIndirect)
                des = "[" + des + "]";

            aSW.WriteLine(string.Format("{0} {2} {1}", Code, des, Const.SizeToString(Size)));
        }
    }
}

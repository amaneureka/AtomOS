/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          DestinationSourceSize type Instructions abstract class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.IO;

namespace Atomix.Assembler.x86
{
    public abstract class DestinationSourceSize : Instruction
    {
        public Registers? DestinationReg;
        public string DestinationRef;
        public bool DestinationIndirect;
        public int? DestinationDisplacement;

        public Registers? SourceReg;
        public string SourceRef;
        public bool SourceIndirect;
        public int? SourceDisplacement;

        public byte Size;

        public DestinationSourceSize(string aMnemonic)
            : base(aMnemonic)  { }

        public override void FlushText(StreamWriter aSW)
        {
            string des = DestinationReg.HasValue ? DestinationReg.ToString() : DestinationRef;
            string src = SourceReg.HasValue ? SourceReg.ToString() : SourceRef;

            if (DestinationDisplacement > 0)
                des = des + " + 0x" + ((uint)DestinationDisplacement).ToString("X");
            else if (DestinationDisplacement < 0)
                des = des + " - 0x" + ((int)(-1 * DestinationDisplacement)).ToString("X");

            if (SourceDisplacement > 0)
                src = src + " + 0x" + ((uint)SourceDisplacement).ToString("X");
            else if (SourceDisplacement < 0)
                src = src + " - 0x" + ((int)(-1 * SourceDisplacement)).ToString("X");

            if (DestinationIndirect)
                des = "[" + des + "]";

            if (SourceIndirect)
                src = "[" + src + "]";

            aSW.WriteLine(string.Format("{0} {3} {1}, {2}", Code, des, src, Const.SizeToString(Size)));
        }
    }
}

/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Movsx x86 instruction
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.IO;

namespace Atomix.Assembler.x86
{
    public class Movsx : DestinationSourceSize
    {
        public Movsx()
            : base("movsx") { }

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

            aSW.WriteLine(string.Format("{0} {1}, {3} {2}", Code, des, src, Const.SizeToString(Size)));
        }
    }
}

/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Conversion type x86 instruction
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.IO;

namespace Atomix.Assembler.x86
{
    public class Conversion : Instruction
    {
        // Based on: http://docs.oracle.com/cd/E19455-01/806-3773/6jct9o0an/index.html

        public ConversionCode Type { get; set; }

        public Conversion()
            : base("conversion") { }

        public override void FlushText(StreamWriter sw)
        {
            switch (Type)
            {
                case ConversionCode.Byte_2_Word:
                    sw.WriteLine("cbtw");
                    break;
                case ConversionCode.Word_2_Long:
                    sw.WriteLine("cwtl");
                    break;
                case ConversionCode.SignedWord_2_SignedDoubleWord:
                    sw.WriteLine("cwtd");
                    break;
                case ConversionCode.SignedLong_2_SignedDoubleLong:
                    sw.WriteLine("cltd");
                    break;
                case ConversionCode.SignedDWord_2_SignedQWord:
                    sw.WriteLine("cdq");
                    break;
            }
        }
    }
}

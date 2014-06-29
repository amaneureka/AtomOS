using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Atomix.CompilerExt;
using Atomix.Assembler;

namespace Atomix.Assembler.x86
{
    //http://docs.oracle.com/cd/E19455-01/806-3773/6jct9o0an/index.html
    public class Conversion : Instruction
    {
        public ConversionCode Code;
        public Conversion()
            : base("conversion") { }

        public override void FlushText(StreamWriter sw)
        {
            switch (Code)
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
                default:
                    throw new Exception("Conversion -> No Code Choosen");
            }            
        }             
    }
    public enum ConversionCode
    {
        /// <summary>
        /// AL -> AX
        /// </summary>
        Byte_2_Word,
        /// <summary>
        /// AX -> EAX
        /// </summary>
        Word_2_Long,
        /// <summary>
        /// AX -> DX:AX
        /// </summary>
        SignedWord_2_SignedDoubleWord,
        /// <summary>
        /// EAX -> EAX:EDX
        /// </summary>
        SignedLong_2_SignedDoubleLong,
        /// <summary>
        /// EAX -> EAX:EDX
        /// </summary>
        SignedDWord_2_SignedQWord
    };
}

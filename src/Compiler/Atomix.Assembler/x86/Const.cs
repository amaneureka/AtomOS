/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          misc function file
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Assembler.x86
{
    public enum ComparePseudoOpcodes : byte
    {
        Equal = 0,
        LessThan = 1,
        LessThanOrEqualTo = 2,
        Unordered = 3,
        NotEqual = 4,
        NotLessThan = 5,
        NotLessThanOrEqualTo = 6,
        Ordered = 7
    };

    public enum ConditionalJumpEnum
    {
        JMP, JO, JNO, JS, JNS, JE, JZ, JNE, JNZ, JB, JNAE, JC,
        JNB, JAE, JNC, JBE, JNA, JA, JNBE, JL, JNGE, JGE, JNL,
        JLE, JNG, JG, JNLE, JP, JPE, JNP, JPO, JCXZ, JECXZ,
    };

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

    public class Const
    {
        public static string SizeToString(byte aSize)
        {
            switch (aSize)
            {
                case 8:
                    return "byte";
                case 16:
                    return "word";
                case 32:
                    return "dword";
                case 64:
                    return "qword";
                default:
                    return "dword";
            }
        }
    }
}

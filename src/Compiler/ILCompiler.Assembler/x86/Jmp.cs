using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.Assembler.x86
{
    public class Jmp : Instruction
    {
        public ConditionalJumpEnum? Condition;
        public string DestinationRef;

        public Jmp()
            : base("jmp")
        { }

        private string[] ConditionArray = new string[] 
        { "jmp ", "jo ", "jno ", "js ", "jns ", "je ", "jz ", "jne ", "jnz ", "jb ", "jnae ", "jc ", 
          "jnb ", "jae ", "jnc ", "jbe ", "jna ", "ja ", "jnbe ", "jl ", "jnge ", "jge ", "jnl ", 
          "jle ", "jng ", "jg ", "jnle ", "jp ", "jpe ", "jnp ", "jpo ", "jcxz ", "jecxz " };

        public override void FlushText(System.IO.StreamWriter sw)
        {
            sw.WriteLine((Condition.HasValue ? ConditionArray[(int)Condition] + "near ": ConditionArray[0]) + DestinationRef);
        }        
    }
    public enum ConditionalJumpEnum : int
    {
        JO = 1, JNO = 2, JS = 3, JNS = 4, JE = 5, JZ = 6, JNE = 7, JNZ = 8, JB = 9, JNAE = 10, JC = 11,
        JNB = 12, JAE = 13, JNC = 14, JBE = 15, JNA = 16, JA = 17, JNBE = 18, JL = 19, JNGE = 20, JGE = 21, JNL = 22,
        JLE = 23, JNG = 24, JG = 25, JNLE = 26, JP = 27, JPE = 28, JNP = 29, JPO = 30, JCXZ = 31, JECXZ = 32
    };
}

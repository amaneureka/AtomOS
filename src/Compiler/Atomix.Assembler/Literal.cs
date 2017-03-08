/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Literal class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.IO;

namespace Atomix.Assembler
{
    public class Literal : Instruction
    {
        public readonly string Assembly;

        public Literal(string aAsm)
            :base ("Literal")
        {
            Assembly = aAsm;
        }

        public Literal(string aFormat, object aObj)
            : base("Literal")
        {
            Assembly = string.Format(aFormat, aObj);
        }

        public Literal(string aFormat, params object[] aObjs)
            : base("Literal")
        {
            Assembly = string.Format(aFormat, aObjs);
        }

        public override void FlushText(StreamWriter aSW)
        {
            aSW.WriteLine(Assembly);
        }
    }
}

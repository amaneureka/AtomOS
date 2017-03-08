/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Literal Assembly Codes
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Machine
{
    public class Literal : Instruction
    {
        string mCode;

        public Literal(string aCode)
            : base(string.Empty)
        {
            mCode = aCode;
        }

        public Literal(string aCode, params object[] aParams)
            : base(string.Empty)
        {
            mCode = string.Format(aCode, aParams);
        }

        public override string ToString()
        {
            return mCode;
        }
    }
}

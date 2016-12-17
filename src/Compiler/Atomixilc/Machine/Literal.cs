/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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

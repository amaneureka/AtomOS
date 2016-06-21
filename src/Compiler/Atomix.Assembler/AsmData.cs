/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Literal Assembly Handling class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.IO;
using System.Linq;

namespace Atomix.Assembler
{
    public class AsmData
    {
        private string mName;
        private string mValue;
        
        public AsmData(string aName, string aValue)
        {
            mName = aName;
            mValue = aValue;
        }

        public AsmData(string aName, uint aValue)
        {
            mName = aName;
            mValue = string.Format("dd {0}", aValue);
        }

        public AsmData(string aName, byte[] aValue)
        {
            mName = aName;
            mValue = string.Format("db {0}", string.Join(", ", aValue.Select(a => a.ToString())));
        }

        public AsmData(string aName, string[] aValue)
        {
            mName = aName;
            mValue = string.Format("dd {0}", string.Join(", ", aValue.Select(a => a.ToString())));
        }
        
        public void FlushText(StreamWriter aSW)
        {
            aSW.WriteLine(string.Format("{0} {1}", mName, mValue));
        }
    }
}

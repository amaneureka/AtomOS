/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Literal Assembly Handling class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.IO;
using System.Linq;

namespace Atomix.Assembler
{
    public class AsmData
    {
        public readonly string Name;
        public readonly string Value;
        public readonly bool IsBssData;

        public AsmData(string aName, string aValue)
        {
            Name = aName;
            Value = aValue;
        }

        public AsmData(string aName, uint aLength)
        {
            Name = aName;
            Value = "resb " + aLength;
            IsBssData = true;
        }

        public AsmData(string aName, byte[] aValue)
        {
            Name = aName;
            if (aValue.LastOrDefault(a => a != 0) == 0)
            {
                IsBssData = true;
                Value = "resb " + aValue.Length;
            }
            else
            {
                Value = string.Format("db {0}", string.Join(", ", aValue.Select(a => a.ToString())));
            }
        }

        public AsmData(string aName, string[] aValue)
        {
            Name = aName;
            Value = string.Format("dd {0}", string.Join(", ", aValue.Select(a => a.ToString())));
            IsBssData = false;
        }

        public void FlushText(StreamWriter aSW)
        {
            aSW.WriteLine(string.Format("{0} {1}", Name, Value));
        }
    }
}

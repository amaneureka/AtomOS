using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Atomix.Assembler
{
    public class AsmData : IComparable
    {
        protected string mName;
        protected byte[] mValue = null;
        protected string[] mValue2 = null;
        protected string aRAW = string.Empty;

        public AsmData(string aName, string aValue)
        {
            this.mName = aName;
            this.aRAW = aValue;
        }

        public AsmData(string aName, byte[] aValue)
        {
            this.mName = aName;
            this.mValue = aValue;
        }

        public AsmData(string aName, string[] aValue)
        {
            this.mName = aName;
            this.mValue2 = aValue;
        }
        

        public void FlushText(StreamWriter sw)
        {
            if (mValue != null)
                sw.WriteLine(string.Format("{0} db {1}", mName, string.Join(", ", ((byte[])mValue).Select(b => string.Format("{0}", b)))));
            else if (mValue2 != null)
            {
                sw.Write(mName);
                sw.Write(" dd ");
                sw.WriteLine(string.Join(", ", (mValue2).Select(a => (a == null ? "0" : a.ToString()))));
            }
            else
                sw.WriteLine(mName + " " + aRAW);
        }

        public int CompareTo(object obj)
        {
            if (obj == null) 
                return 1;

            var x = obj as AsmData;
            if (x != null)
                return this.mName.CompareTo(x.mName);

            throw new Exception("Unknown Exception");
        }
    }
}

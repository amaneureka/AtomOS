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

        public AsmData(string aName, object[] aValue)
        {
            byte[] result = new byte[aValue.Length * sizeof(int)];
            Buffer.BlockCopy(aValue, 0, result, 0, result.Length);
            this.mName = aName;
            this.mValue = result;            
        }
        

        public void FlushText(StreamWriter sw)
        {
            if (mValue != null)
                sw.WriteLine(string.Format("{0} db {1}", mName, string.Join(", ", ((byte[])mValue).Select(b => string.Format("{0}", b)))));
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

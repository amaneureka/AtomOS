using System;
using System.Collections.Generic;

namespace libAtomixH.mscorlib.Text
{
    public class StringBuilder
    {
        private List<char> xTemp;

        public StringBuilder ()
        {
            xTemp = new List<char> ();
        }

        public void Clear ()
        {
            xTemp = new List<char> ();
        }

        public void Append (char xChar)
        {
            xTemp.Add (xChar);
        }

        public void Append (string Str)
        {
            for (int i = 0; i < Str.Length; i++)
                xTemp.Add (Str[i]);
        }

        public void CutOne ()
        {
            List<char> newTemp = new List<char> ();

            for (int i = 0; i < xTemp.Count - 1; i++)
            {
                newTemp.Add (xTemp[i]);
            }

            xTemp = newTemp;
        }

        public void InsertAt (int pos, char chr)
        {
            List<char> newTemp = new List<char> ();

            for (int i = 0; i< xTemp.Count + 1; i++)
            {
                if (i < pos)
                {
                    newTemp.Add (xTemp[i]);
                }
                if (i == pos)
                {
                    newTemp.Add (chr);
                }
                if (i > pos)
                {
                    newTemp.Add (xTemp[i - 1]);
                }
            }

            xTemp = newTemp;
        }

        public void UpdateAt (int pos, char chr)
        {
            xTemp[pos] = chr;
        }

        public void RemoveAt (int pos)
        {
            List<char> newTemp = new List<char> ();

            for (int i = 0; i < xTemp.Count; i++)
            {
                if (i < pos)
                    newTemp.Add (xTemp[i]);
                if (i > pos)
                    newTemp.Add (xTemp[i]);
            }

            xTemp = newTemp;
        }

        public string Flush ()
        {
            //Calling two time count use more memory so save it as local variable
            var len = xTemp.Count;

            char[] xResult = new char[len];
            for (int i = 0; i < len; i++)
            {
                xResult[i] = xTemp[i];
            }

            return new String (xResult);
        }
    }
}

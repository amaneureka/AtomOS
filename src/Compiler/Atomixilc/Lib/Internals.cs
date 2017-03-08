/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Application Internals support functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Lib
{
    public static class Internals
    {
        #region GetHashCode
        //http://www.cse.yorku.ca/~oz/hash.html
        public static uint GetHashCode(string aData)
        {
            uint Hash = 0;

            int index = 0, length = aData.Length;
            while (index < length)
                Hash = aData[index++] + (Hash << 16) + (Hash << 6) - Hash;
            return Hash;
        }

        public static uint GetHashCode(uint aData)
        {
            return aData;
        }

        public static uint GetHashCode(char[] aData)
        {
            uint Hash = 0;

            int index = 0, length = aData.Length;
            while (index < length)
                Hash = aData[index++] + (Hash << 16) + (Hash << 6) - Hash;
            return Hash;
        }

        public static uint GetHashCode(byte[] aData)
        {
            uint Hash = 0;
            
            int index = 0, length = aData.Length;
            while (index < length)
                Hash = aData[index++] + (Hash << 16) + (Hash << 6) - Hash;
            return Hash;
        }

        #endregion

        #region Equals
        public static bool Equals(uint a, uint b)
        {
            return (a == b);
        }
        #endregion
    }
}

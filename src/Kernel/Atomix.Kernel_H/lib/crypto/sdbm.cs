/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Sdbm Hashing Function
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.lib.crypto
{
    // Source: http://www.cse.yorku.ca/~oz/hash.html
    internal static class sdbm
    {
        internal static uint GetHashCode(this string aData)
        {
            return GetHashCode(aData, 0);
        }

        internal static uint GetHashCode(this string aData, uint aSeed)
        {
            uint Hash = aSeed;

            int index = 0, length = aData.Length;
            while (index < length)
                Hash = aData[index++] + (Hash << 16) + (Hash << 6) - Hash;
            return Hash;
        }

        internal static uint GetHashCode(this char[] aData)
        {
            uint Hash = 0;

            int index = 0, length = aData.Length;
            while (index < length)
                Hash = aData[index++] + (Hash << 16) + (Hash << 6) - Hash;
            return Hash;
        }

        internal static uint GetHashCode(this byte[] aData)
        {
            uint Hash = 0;

            int index = 0, length = aData.Length;
            while (index < length)
                Hash = aData[index++] + (Hash << 16) + (Hash << 6) - Hash;
            return Hash;
        }
    }
}

using System;

namespace Atomix.Kernel_H.lib.crypto
{
    //Source: http://www.cse.yorku.ca/~oz/hash.html
    public static class sdbm
    {
        public static uint GetsdbmHash(this string str, uint Seed = 0)
        {
            uint Hash = Seed;

            int index = 0, length = str.Length;
            while (index < length)
                Hash = str[index++] + (Hash << 16) + (Hash << 6) - Hash;
            return Hash;
        }

        public static uint GetsdbmHash(this char[] aData)
        {
            uint Hash = 0;

            int index = 0, length = aData.Length;
            while (index < length)
                Hash = aData[index++] + (Hash << 16) + (Hash << 6) - Hash;
            return Hash;
        }

        public static uint GetsdbmHash(this byte[] aData)
        {
            uint Hash = 0;

            int index = 0, length = aData.Length;
            while (index < length)
                Hash = aData[index++] + (Hash << 16) + (Hash << 6) - Hash;
            return Hash;
        }
    }
}

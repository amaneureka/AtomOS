/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* Copyright (c) 2015, Atomix Development, Inc - All Rights Reserved                                        *
*                                                                                                          *
* Unauthorized copying of this file, via any medium is strictly prohibited                                 *
* Proprietary and confidential                                                                             *
* Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2015                                       *
*                                                                                                          *
*   Namespace     ::  Atomix.Kernel_H.lib.crypto                                                           *
*   File          ::  sdbm.cs                                                                              *
*                                                                                                          *
*   Description                                                                                            *
*       SDBM Hashing Functions                                                                             *
*                                                                                                          *
*   History                                                                                                *
*       20-12-2015      Aman Priyadarshi      Added Method                                                 *
*       04-01-2016      Aman Priyadarshi      Added Seed option                                            *
*       24-03-2016      Aman Priyadarshi      Added File Header and Typos                                  *
*                                                                                                          *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;

namespace Atomix.Kernel_H.lib.crypto
{
    //Source: http://www.cse.yorku.ca/~oz/hash.html
    public static class sdbm
    {
        public static uint GetHashCode(this string aData)
        {
            return GetHashCode(aData, 0);
        }

        public static uint GetHashCode(this string aData, uint aSeed)
        {
            uint Hash = aSeed;

            int index = 0, length = aData.Length;
            while (index < length)
                Hash = aData[index++] + (Hash << 16) + (Hash << 6) - Hash;
            return Hash;
        }

        public static uint GetHashCode(this char[] aData)
        {
            uint Hash = 0;

            int index = 0, length = aData.Length;
            while (index < length)
                Hash = aData[index++] + (Hash << 16) + (Hash << 6) - Hash;
            return Hash;
        }

        public static uint GetHashCode(this byte[] aData)
        {
            uint Hash = 0;

            int index = 0, length = aData.Length;
            while (index < length)
                Hash = aData[index++] + (Hash << 16) + (Hash << 6) - Hash;
            return Hash;
        }
    }
}

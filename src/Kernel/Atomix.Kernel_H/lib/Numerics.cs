/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Nunerics Helper Functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.lib
{
    internal class Numerics
    {
        internal static uint ParseHex(string str)
        {
            uint ans = 0;
            int temp, index = 0, len = str.Length;
            while(index < len && (temp = str[index++]) >= '0')
            {
                ans <<= 0x4;
                if (temp <= '9')
                    ans += (uint)(temp - '0');
                else if (temp <= 'F')
                    ans += (uint)(temp - 'A') + 10;
                else
                    ans += (uint)(temp - 'a') + 10;
            }
            return ans;
        }
    }
}

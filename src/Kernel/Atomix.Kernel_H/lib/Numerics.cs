using System;

namespace Atomix.Kernel_H.lib
{
    public class Numerics
    {
        public static uint ParseHex(string str)
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

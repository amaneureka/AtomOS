/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Atomix.Core;

namespace Atomix.Shell
{
    [Application(CPUArch.x86)]
    public class Startx86
    {
        internal static int AddNums2(int a, int b)
        {
            return (a + b);
        }

        [Plug("lol")]
        public static int AddNums(int a, int b)
        {
            return (a + b*2);
        }

        public static void main(char[] args)
        {
            AddNums(0, 0);
            while (true) ;
        }
    }
}

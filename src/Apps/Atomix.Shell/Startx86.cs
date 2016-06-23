/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is strictly prohibited
*                   Proprietary and confidential
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
        public static int AddNums2(int a, int b)
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

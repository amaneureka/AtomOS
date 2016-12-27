/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Marshal extension functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;

namespace Atomix.Kernel_H.Lib
{
    internal class Marshal
    {
        internal static unsafe void Copy(char* aDes, string aSrc, int aLen)
        {
            uint src = Native.GetContentAddress(aSrc);
            Memory.FastCopy((uint)aDes, src, (uint)(aLen * sizeof(char)));
            aDes[aLen] = '\0';
        }
    }
}

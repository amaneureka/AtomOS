/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Marshal extension functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Arch.x86;

namespace Atomix.Kernel_H.Lib
{
    internal class Marshal
    {
        internal static unsafe void Copy(string aDes, char* aSrc, uint aLen)
        {
            uint des = Native.GetContentAddress(aDes);
            Memory.FastCopy(des, (uint)aSrc, aLen);
            aSrc[aLen] = '\0';
        }
    }
}

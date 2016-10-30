/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Delegates extension functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Assembler;
using Atomix.Assembler.x86;

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.Lib
{
    internal static class Delegates
    {
        [Assembly(true)]
        internal static uint GetAddress(this Action aDelegate)
        {
            // [aDelegate + 0xC] := Address Field
            AssemblyHelper.AssemblerCode.Add(new Mov
                {
                    DestinationReg = Registers.EBX,
                    SourceReg = Registers.EBP,
                    SourceDisplacement = 0x8,
                    SourceIndirect = true
                });

            // EAX := [Address Field]
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBX, SourceDisplacement = 0xC, SourceIndirect = true });
            // Return : [EBP + 0x8] = EAX
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.EAX, DestinationDisplacement = 0x8, DestinationIndirect = true });

            return 0;
        }
    }
}

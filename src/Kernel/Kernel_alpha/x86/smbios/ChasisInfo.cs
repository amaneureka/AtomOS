using System;
using System.Collections.Generic;
using Kernel_alpha.x86;

namespace Kernel_alpha.x86.smbios
{
    public unsafe class ChasisInfo : Entry
    {
        public ChasisInfo(SMBIOS.SMBIOSHeader* Header)
            : base(Header)
        {            
            var strings = GetAllStrings(5);
        }
    }
}

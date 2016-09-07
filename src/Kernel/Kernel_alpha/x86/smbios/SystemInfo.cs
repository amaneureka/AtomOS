using System;
using System.Collections.Generic;
using Kernel_alpha.x86;

namespace Kernel_alpha.x86.smbios
{
    public unsafe class SystemInfo : Entry
    {
        public SystemInfo(SMBIOS.SMBIOSHeader* Header)
            : base(Header)
        {
            var strings = GetAllStrings(6);
        }
    }
}

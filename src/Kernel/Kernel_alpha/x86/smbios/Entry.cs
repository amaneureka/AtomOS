using System;
using System.Collections.Generic;
using Kernel_alpha.x86;

namespace Kernel_alpha.x86.smbios
{
    public abstract unsafe class Entry
    {
        protected SMBIOS.SMBIOSHeader* Header;
        protected uint aTotalLength;

        public uint TotalLength
        {
            get { return aTotalLength; }
        }

        public Entry(SMBIOS.SMBIOSHeader* aHeader)
        {
            this.Header = aHeader;
            this.aTotalLength = Header->Length;
        }

        public List<string> GetAllStrings(int count)
        {
            if (count == 0)
                return null;

            var xResult = new List<string>();

            byte* Mem = (byte*)((uint)Header + Header->Length);

            char[] xTemp = new char[64];
            uint p = 0, t = 0;
            while(count > 0)
            {
                if ((xTemp[t++] = (char)Mem[p++]) == 0)
                {
                    aTotalLength += t;
                    count--;
                    t = 0;
                    xResult.Add(new string(xTemp).Trim('\0'));
                }
            }

            return xResult;
        }
    }
}

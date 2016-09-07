using System;
using System.Collections.Generic;
using Kernel_alpha.x86;

namespace Kernel_alpha.x86.smbios
{
    public unsafe class ProcessorInfo : Entry
    {
        protected string mVendorName;
        protected string mVersion;
        protected string mSocket;
        protected uint mSpeed;

        public ProcessorInfo(SMBIOS.SMBIOSHeader* Header)
            : base(Header)
        {
            var strings = GetAllStrings(3);
            mVersion = strings[0];
            mSocket = strings[1];
            mVendorName = strings[2];

            mSpeed = *((ushort*)((uint)Header + 0x16));

            Console.WriteLine("Processor Information---->");
            Console.WriteLine("Vendor Name  ::" + mVendorName);
            Console.WriteLine("Version      ::" + mVersion);
            Console.WriteLine("Socket       ::" + mSocket);
            Console.WriteLine("Speed        ::" + mSpeed.ToString());
        }
    }
}

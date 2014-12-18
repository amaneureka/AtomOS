using System;
using System.Collections.Generic;
using Kernel_alpha.x86;

namespace Kernel_alpha.x86.smbios
{
    public unsafe class BIOSInfo : Entry
    {
        protected string mVendorName;
        protected string mBIOSVersion;
        protected string mBIOSDate;

        public BIOSInfo(SMBIOS.SMBIOSHeader* Header)
            : base(Header)
        {
            /*
             * db 0 ; Indicates BIOS Structure Type            |
             * db 13h ; Length of information in bytes         | HEADER
             * dw ? ; Reserved for handle                      |
             * 
             * db 01h ; String 1 is the Vendor Name            |
             * db 02h ; String 2 is the BIOS version           |
             * dw 0E800h ; BIOS Starting Address               |
             * db 03h ; String 3 is the BIOS Build Date        | DATA
             * db 1 ; Size of BIOS ROM is 128K (64K * (1 + 1)) |
             * dq BIOS_Char ; BIOS Characteristics             |
             * db 0 ; BIOS Characteristics Extension Byte 1    |
             * 
             * db ‘System BIOS Vendor Name’,0 ;                |
             * db ‘4.04’,0 ;                                   | STRINGS
             * db ‘00/00/0000’,0 ;                             |
             * 
             * db 0 ; End of structure
             */


            var strings = GetAllStrings(3);

            mVendorName = strings[0];
            mBIOSVersion = strings[1];
            mBIOSDate = strings[2];

            Console.WriteLine("BIOS Information---->");
            Console.WriteLine("Vendor Name  ::" + mVendorName);
            Console.WriteLine("BIOS Version ::" + mBIOSVersion);
            Console.WriteLine("BIOS Date    ::" + mBIOSDate);
        }
    }
}

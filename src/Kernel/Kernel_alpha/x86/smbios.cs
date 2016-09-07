using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Kernel_alpha.x86.smbios;

namespace Kernel_alpha.x86
{
    public static unsafe class SMBIOS
    {
        private static int Address;
        private static SMBIOSEntryPoint* EntryPoint;
        private static List<Entry> Entries;

        //http://www.dmtf.org/sites/default/files/standards/documents/DSP0134_2.7.1.pdf
        public static void Setup()
        {
            if (!FindTable())
                return;

            Console.WriteLine("smbios found :)");
            EntryPoint = (SMBIOSEntryPoint*)Address;
            Entries = new List<Entry>();
            ParseTables();
        }

        private static unsafe bool FindTable()
        {
            byte* Memory = (byte*)0xF0000;
            int len, i;
            byte checksum;
            while ((uint)Memory < 0x100000)
            {
                if (Memory[0] == (byte)'_' &&
                    Memory[1] == (byte)'S' &&
                    Memory[2] == (byte)'M' &&
                    Memory[3] == (byte)'_')
                {
                    len = Memory[5];
                    checksum = 0;
                    for (i = 0; i < len; i++)
                    {
                        checksum += Memory[i];
                    }

                    if (checksum == 0)
                    {
                        Address = (int)Memory;
                        return true;
                    }
                }
                Memory += 16;
            }
            return false;
        }
        /// <summary>
        /// Not Completed Yet :(
        /// </summary>
        private static void ParseTables()
        {
            SMBIOSHeader* Header;
            Entry ent;
            int count = EntryPoint->NumberOfStructures;
            for (uint i = EntryPoint->TableAddress; (i < EntryPoint->TableAddress + EntryPoint->TableLength) && count > 0;)
            {
                Header = (SMBIOSHeader*)i;
                switch((HeaderType)Header->Type)
                {
                    case HeaderType.BIOS_Info:
                        {
                            ent = new BIOSInfo(Header);
                            i += ent.TotalLength + 2;
                            count--;
                        }
                        break;
                    case HeaderType.System_Info:
                        {
                            ent = new SystemInfo(Header);
                            i += ent.TotalLength + 2;
                            count--;
                        }
                        break;
                    case HeaderType.MainBoard_Info:
                        {
                            ent = new MainBoardInfo(Header);
                            i += ent.TotalLength + 2;
                            count--;
                        }
                        break;
                    case HeaderType.Chasis_Info:
                        {
                            ent = new ChasisInfo(Header);
                            i += ent.TotalLength + 2;
                            count--;
                        }
                        break;
                    case HeaderType.Processor_Info:
                        {
                            ent = new ProcessorInfo(Header);
                            i += ent.TotalLength + 2;
                            count--;
                        }
                        break;
                }
            }
        }

        #region Struct

        [StructLayout(LayoutKind.Explicit, Size = 31)]
        public unsafe struct SMBIOSEntryPoint
        {
            [FieldOffset(0)]
            public fixed byte EntryPointString[4];      //This is _SM_
            [FieldOffset(4)]
            public byte Checksum;                       //This value summed with all the values of the table, should be 0 (overflow)
            [FieldOffset(5)]
            public byte Length;                         //Length of the Entry Point Table. Since version 2.1 of SMBIOS, this is 0x1F
            [FieldOffset(6)]
            public byte MajorVersion;                   //Major Version of SMBIOS
            [FieldOffset(7)]
            public byte MinorVersion;                   //Minor Version of SMBIOS
            [FieldOffset(8)]
            public ushort MaxStructureSize;             //Maximum size of a SMBIOS Structure (we will se later)
            [FieldOffset(10)]
            public byte EntryPointRevision;             //...
            [FieldOffset(11)]
            public fixed byte FormattedArea[5];         //...
            [FieldOffset(16)]
            public fixed byte EntryPointString2[5];     //This is _DMI_
            [FieldOffset(21)]
            public byte Checksum2;                      //Checksum for values from EntryPointString2 to the end of table
            [FieldOffset(22)]
            public ushort TableLength;                  //Length of the Table containing all the structures
            [FieldOffset(24)]
            public uint TableAddress;                   //Address of the Table
            [FieldOffset(28)]
            public ushort NumberOfStructures;           //Number of structures in the table
            [FieldOffset(30)]
            public byte BCDRevision;                    //Unused
        };

        [StructLayout(LayoutKind.Explicit, Size = 4)]
        public struct SMBIOSHeader
        {
            [FieldOffset(0)]
            public byte Type;
            [FieldOffset(1)]
            public byte Length;
            [FieldOffset(2)]
            public ushort Handle;
        };

        public enum HeaderType
        {
            BIOS_Info = 0,
            System_Info = 1,
            MainBoard_Info = 2,
            Chasis_Info = 3,
            Processor_Info = 4,
            Cache_Info = 7,
            SystemSlot_Info = 9,
            PhysicalMem_Array = 16,
            MemoryDevice_Info = 17,
            MemoryDevice_Mapped = 19,
            SystemBoot_Info = 32
        };
        #endregion
    }
}

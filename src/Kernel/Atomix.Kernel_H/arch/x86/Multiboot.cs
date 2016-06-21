/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Multiboot Information Parsing
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.core;

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.arch.x86
{
    public static unsafe class Multiboot
    {
        const uint MULTIBOOT_MEMORY_AVAILABLE = 1;
        const uint MULTIBOOT_MEMORY_RESERVED = 2;

        static Multiboot_Info* Mb_Info;
        static bool aIsValid;
        static uint Initrd;
        static uint InitrdSize;

        public static bool IsValid
        { get { return aIsValid; } }

        public static uint VBE_Control_Info
        {
            get { return Mb_Info->vbeControlInfo; }
        }

        public static uint VBE_Mode_Info
        {
            get { return Mb_Info->vbeModeInfo; }
        }

        public static uint RAM
        {
            get { return (Mb_Info->mem_upper + Mb_Info->mem_lower) * 1024; }
        }

        public static uint RamDisk
        {
            get { return Initrd; }
        }

        public static uint RamDiskSize
        {
            get { return InitrdSize; }
        }

        public static uint RamDiskEnd
        {
            get { return Initrd + InitrdSize; }
        }
        
        public static unsafe void Setup(uint xSig, uint Address)
        {
            if (xSig != 0x2BADB002)
            {
                aIsValid = false;
                Debug.Write("Invalid Multiboot Signature\n");
                return;
            }
            aIsValid = true;
            Address += 0xC0000000;// We are in higher half =D
            Mb_Info = (Multiboot_Info*)Address;
            Debug.Write("Multiboot Found!!\n");
            Debug.Write("       Address       ::%d\n", Address);
            Debug.Write("       VBEModeInfo   ::%d\n", Mb_Info->vbeModeInfo);
            Debug.Write("       VBEControlInfo::%d\n", Mb_Info->vbeControlInfo);

            Initrd = 0;
            InitrdSize = 0;
            var modules = (UInt32*)(Mb_Info->mods_addr + 0xC0000000);
            Debug.Write("       Modules Count:%d\n", Mb_Info->mods_count);
            if (Mb_Info->mods_count > 0)
            {
                Initrd = modules[0];
                InitrdSize = modules[1] - Initrd;
                Initrd += 0xC0000000;
                Debug.Write("       RamDisk:%d\n", Initrd);
                Debug.Write("       RamDisk-Size:%d\n", InitrdSize);
            }
            if (Mb_Info->mods_count == 0)
                Debug.Write("       No Initial RAM Disk Found!!\n");
            Debug.Write("       Flags:%d\n", Mb_Info->Flags);
        }

        public static void DetectMemory()
        {
            if ((Mb_Info->Flags & (0x1<<6)) != 0)
            {
                Debug.Write("Parsing Memory Map\n");
                MemoryMap_Info* mmap = (MemoryMap_Info*)(Mb_Info->memMapAddress + 0xC0000000);

                uint EndAddress = Mb_Info->memMapAddress + 0xC0000000 + Mb_Info->memMapLength;
                while((uint)mmap < EndAddress)
                {
                    if (mmap->Type == MULTIBOOT_MEMORY_RESERVED)
                    {
                        // Let's assume High part is 0 always, because we are running on 32bit CPU
                        for (uint index = 0; index < mmap->Length_Low; index += 0x1000)// Page size
                        {
                            uint Address = mmap->BaseAddress_Low + index;
                            Debug.Write("Marking Address: %d\n", Address);
                            Paging.SetFrame((Address & 0xFFFFF000) / 0x1000);
                        }
                    }
                    mmap = (MemoryMap_Info*)((uint)mmap + sizeof(MemoryMap_Info) + mmap->Size);
                }
            }
        }

        #region Struct
        /// <summary>
        /// For now it is fine, http://git.savannah.gnu.org/cgit/grub.git/tree/doc/multiboot.h?h=multiboot
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 88)]
        public unsafe struct Multiboot_Info
        {
            /* Multiboot info version number */
            [FieldOffset(0)]
            public uint Flags;

            /* Available memory from BIOS */
            [FieldOffset(4)]
            public uint mem_lower;
            [FieldOffset(8)]
            public uint mem_upper;

            /* "root" partition */
            [FieldOffset(12)]
            public uint boot_device;

            /* Kernel command line */
            [FieldOffset(16)]
            public uint cmdline;

            /* Boot-Module list */
            [FieldOffset(20)]
            public uint mods_count;
            [FieldOffset(24)]
            public uint mods_addr;

            /* if bits 4 or 5 in flags are set */
            [FieldOffset(28)]
            public fixed UInt32 syms[4];
            /* if bit 6 in flags is set */
            [FieldOffset(44)]
            public UInt32 memMapLength;
            /* if bit 6 in flags is set */
            [FieldOffset(48)]
            public UInt32 memMapAddress;
            /* if bit 7 in flags is set */
            [FieldOffset(52)]
            public UInt32 drivesLength;
            /* if bit 7 in flags is set */
            [FieldOffset(56)]
            public UInt32 drivesAddress;
            /* if bit 8 in flags is set */
            [FieldOffset(60)]
            public UInt32 configTable;

            //Bootloader name =D

            /* if bit 9 in flags is set */
            [FieldOffset(68)]
            public UInt32 apmTable;
            /* if bit 10 in flags is set */
            [FieldOffset(72)]
            public UInt32 vbeControlInfo;
            /* if bit 11 in flags is set */
            [FieldOffset(76)]
            public UInt32 vbeModeInfo;
            /* all vbe_* set if bit 12 in flags are set */
            [FieldOffset(80)]
            public UInt16 vbeMode;
            [FieldOffset(82)]
            public UInt16 vbeInterfaceSeg;
            [FieldOffset(84)]
            public UInt16 vbeInterfaceOff;
            [FieldOffset(86)]
            public UInt16 vbeInterfaceLength;
        }

        [StructLayout(LayoutKind.Explicit, Size = 24)]
        public unsafe struct MemoryMap_Info
        {
            [FieldOffset(0)]
            public UInt32 Size;
            [FieldOffset(4)]
            public UInt32 BaseAddress_Low;
            [FieldOffset(8)]
            public UInt32 BaseAddress_High;
            [FieldOffset(12)]
            public UInt64 Length_Low;
            [FieldOffset(16)]
            public UInt64 Length_High;
            [FieldOffset(20)]
            public UInt32 Type;
        }
        #endregion
    }
}

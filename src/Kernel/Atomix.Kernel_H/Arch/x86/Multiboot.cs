/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Multiboot Information Parsing
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.Arch.x86
{
    /// <summary>
    /// For now it is fine, http://git.savannah.gnu.org/cgit/grub.git/tree/doc/multiboot.h?h=multiboot
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 88)]
    internal unsafe struct Multiboot_Info
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
        public fixed uint syms[4];
        /* if bit 6 in flags is set */
        [FieldOffset(44)]
        public uint memMapLength;
        /* if bit 6 in flags is set */
        [FieldOffset(48)]
        public uint memMapAddress;
        /* if bit 7 in flags is set */
        [FieldOffset(52)]
        public uint drivesLength;
        /* if bit 7 in flags is set */
        [FieldOffset(56)]
        public uint drivesAddress;
        /* if bit 8 in flags is set */
        [FieldOffset(60)]
        public uint configTable;

        // Bootloader name =D

        /* if bit 9 in flags is set */
        [FieldOffset(68)]
        public uint apmTable;
        /* if bit 10 in flags is set */
        [FieldOffset(72)]
        public uint vbeControlInfo;
        /* if bit 11 in flags is set */
        [FieldOffset(76)]
        public uint vbeModeInfo;
        /* all vbe_* set if bit 12 in flags are set */
        [FieldOffset(80)]
        public ushort vbeMode;
        [FieldOffset(82)]
        public ushort vbeInterfaceSeg;
        [FieldOffset(84)]
        public ushort vbeInterfaceOff;
        [FieldOffset(86)]
        public ushort vbeInterfaceLength;
    }

    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public unsafe struct MemoryMap_Info
    {
        [FieldOffset(0)]
        public uint Size;
        [FieldOffset(4)]
        public uint BaseAddress_Low;
        [FieldOffset(8)]
        public uint BaseAddress_High;
        [FieldOffset(12)]
        public uint Length_Low;
        [FieldOffset(16)]
        public uint Length_High;
        [FieldOffset(20)]
        public uint Type;
    }

    internal static unsafe class Multiboot
    {
        const uint MULTIBOOT_MEMORY_AVAILABLE = 1;
        const uint MULTIBOOT_MEMORY_RESERVED = 2;

        static Multiboot_Info* Mb_Info;
        static bool aIsValid;
        static uint Initrd;
        static uint InitrdSize;

        internal static bool IsValid
        { get { return aIsValid; } }

        internal static uint VBE_Control_Info
        {
            get { return Mb_Info->vbeControlInfo; }
        }

        internal static uint VBE_Mode_Info
        {
            get { return Mb_Info->vbeModeInfo; }
        }

        internal static uint RAM
        {
            get { return (Mb_Info->mem_upper + Mb_Info->mem_lower) * 1024; }
        }

        internal static uint RamDisk
        {
            get { return Initrd; }
        }

        internal static uint RamDiskSize
        {
            get { return InitrdSize; }
        }

        internal static uint RamDiskEnd
        {
            get { return Initrd + InitrdSize; }
        }

        internal static unsafe void Setup(uint xSig, uint Address)
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

        internal static void DetectMemory()
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
                    mmap = (MemoryMap_Info*)((uint)mmap + (uint)sizeof(MemoryMap_Info) + mmap->Size);
                }
            }
        }
    }
}

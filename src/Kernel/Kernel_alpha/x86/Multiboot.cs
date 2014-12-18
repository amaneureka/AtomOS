using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kernel_alpha.x86.Intrinsic;
using System.Runtime.InteropServices;

namespace Kernel_alpha.x86
{
    public static unsafe class Multiboot
    {
        private const uint MULTIBOOT_BOOTLOADER_MAGIC = 0x2BADB002;

        private static uint MultibootHeader;

        private static Multiboot_Info* Mb_Info;
        public static void Setup(uint magic, uint address)
        {
            if (magic != MULTIBOOT_BOOTLOADER_MAGIC)
            {
                Console.WriteLine("No VALID MULTIBOOT");
                while (true) ;
            }
            MultibootHeader = address;
            Mb_Info = (Multiboot_Info*)MultibootHeader;
        }

        #region Info
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
        #endregion

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
            public UInt32 vbeMode;
            [FieldOffset(82)]
            public UInt32 vbeInterfaceSeg;
            [FieldOffset(84)]
            public UInt32 vbeInterfaceOff;
            [FieldOffset(86)]
            public UInt32 vbeInterfaceLength;
        }
        #endregion
    }
}

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
        private static uint MultibootHeader;

        /// <summary>
        /// For now it is fine, http://git.savannah.gnu.org/cgit/grub.git/tree/doc/multiboot.h?h=multiboot
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct Multiboot_Info
        {
            /* Multiboot info version number */
            public uint Flags;

            /* Available memory from BIOS */
            public uint mem_lower;
            public uint mem_upper;

            /* "root" partition */
            public uint boot_device;

            /* Kernel command line */
            public uint cmdline;

            /* Boot-Module list */
            public uint mods_count;
            public uint mods_addr;
        }

        private static Multiboot_Info* Mb_Info;

        public const uint MULTIBOOT_BOOTLOADER_MAGIC = 0x2BADB002;
        public static void Setup(uint magic, uint address)
        {
            if (magic != MULTIBOOT_BOOTLOADER_MAGIC)
                return;            
            MultibootHeader = address;
            Mb_Info = (Multiboot_Info*)MultibootHeader;
        }

        public static uint Address
        {
            get
            {
                return MultibootHeader;
            }
        }

        public static uint RAM
        {
            get
            {
                return (Mb_Info->mem_upper + Mb_Info->mem_lower) * 1024;
            }
        }
    }
}

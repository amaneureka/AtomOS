using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha.x86
{
    public static class Multiboot
    {
        public static uint MultibootStructure;

        /// <summary>
        /// Standard Multiboot Address
        /// </summary>
        public const uint MagicNumber = 0x1BADB002;
        /// <summary>
        /// Standard Multiboot Magic Number Address
        /// </summary>
        public const uint MultiBootMagicAddress = 0x200000;
        /// <summary>
        /// Standard Multiboot Structure pointer Address
        /// </summary>
        public const uint MultiBootStructAddress = 0x200004;
        /// <summary>
        /// Is ValidMultiboot Magic Number
        /// </summary>
        public static bool IsValidMultiboot;

        public static void Setup()
        {
            MultibootStructure = 0;
            IsValidMultiboot =  LocateMultiboot();
        }

        private static bool LocateMultiboot()
        {
            uint _magic = Native.Read32(MultiBootMagicAddress);
            uint _ptr = Native.Read32(MultiBootStructAddress);
            
            if (_magic != MagicNumber) 
                return false; //Wrong Multiboot Address

            MultibootStructure = _ptr; //Set Structure pointer

            return true;
        }

        public static uint GetRAM
        {
            get
            {
                return Native.Read32(MultiBootStructAddress + 0x4);
            }
        }
    }
}

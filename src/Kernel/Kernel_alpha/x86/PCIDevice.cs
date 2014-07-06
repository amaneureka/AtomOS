using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha.x86
{
    public class PCIDevice
    {
        public uint Bus;
        public uint Slot;
        public uint Function;

        public readonly ushort VendorID;
        public readonly ushort DeviceID;

        public readonly byte RevisionID;
        public readonly byte ProgIF;
        public readonly byte Subclass;
        public readonly byte ClassCode;

        public readonly byte CacheLineSize;
        public readonly byte LatencyTimer;
        public readonly byte InterruptLine;

        public readonly bool DeviceExists;

        public readonly PCIHeaderType HeaderType;
        public readonly PCIBist BIST;
        public readonly PCIInterruptPIN InterruptPIN;

        public const ushort ConfigAddressPort = 0xCF8;
        public const ushort ConfigDataPort = 0xCFC;

        public PCIDevice(uint bus, uint slot, uint function)
        {
            this.Bus = bus;
            this.Slot = slot;
            this.Function = function;

            VendorID = ReadRegister16(0x00);
            DeviceID = ReadRegister16(0x02);
            
            RevisionID = ReadRegister8(0x08);
            ProgIF = ReadRegister8(0x09);
            Subclass = ReadRegister8(0x0A);
            ClassCode = ReadRegister8(0x0B);

            CacheLineSize = ReadRegister8(0x0C);
            LatencyTimer = ReadRegister8(0x0D);
            HeaderType = (PCIHeaderType)ReadRegister8(0x0E);
            BIST = (PCIBist)ReadRegister8(0x0F);

            InterruptLine = ReadRegister8(0x3C);
            InterruptPIN = (PCIInterruptPIN)ReadRegister8(0x3D);

            DeviceExists = (uint)VendorID != 0xFFFF && (uint)DeviceID != 0xFFFF;
        }

        #region IO Port
        protected byte ReadRegister8(byte aRegister)
        {
            UInt32 xAddr = GetAddressBase(Bus, Slot, Function) | ((UInt32)(aRegister & 0xFC));            
            Native.Out32(ConfigAddressPort, xAddr);
            return (byte)(Native.In32(ConfigDataPort) >> ((aRegister % 4) * 8) & 0xFF);
        }

        protected void WriteRegister8(byte aRegister, byte value)
        {
            UInt32 xAddr = GetAddressBase(Bus, Slot, Function) | ((UInt32)(aRegister & 0xFC));
            Native.Out32(ConfigAddressPort, xAddr);
            Native.Out8(ConfigDataPort, value);
        }

        protected UInt16 ReadRegister16(byte aRegister)
        {
            UInt32 xAddr = GetAddressBase(Bus, Slot, Function) | ((UInt32)(aRegister & 0xFC));
            Native.Out32(ConfigAddressPort, xAddr);
            return (UInt16)(Native.In32(ConfigDataPort) >> ((aRegister % 4) * 8) & 0xFFFF); ;
        }

        protected void WriteRegister16(byte aRegister, ushort value)
        {
            UInt32 xAddr = GetAddressBase(Bus, Slot, Function) | ((UInt32)(aRegister & 0xFC));
            Native.Out32(ConfigAddressPort, xAddr);            
            Native.Out16(ConfigDataPort, value);
        }

        protected UInt32 ReadRegister32(byte aRegister)
        {
            UInt32 xAddr = GetAddressBase(Bus, Slot, Function) | ((UInt32)(aRegister & 0xFC));
            Native.Out32(ConfigAddressPort, xAddr);
            return Native.In32(ConfigDataPort);
        }

        protected void WriteRegister32(byte aRegister, uint value)
        {
            UInt32 xAddr = GetAddressBase(Bus, Slot, Function) | ((UInt32)(aRegister & 0xFC));
            Native.Out32(ConfigAddressPort, xAddr);
            Native.Out32(ConfigDataPort, value);
        }
        #endregion
        protected UInt32 GetAddressBase(uint aBus, uint aSlot, uint aFunction)
        {
            // 31 	        30 - 24    23 - 16      15 - 11 	    10 - 8 	          7 - 2 	        1 - 0
            // Enable Bit 	Reserved   Bus Number 	Device Number 	Function Number   Register Number 	00 
            return (UInt32)(
                // Enable bit - must be set
                0x80000000
                // Bits 23-16
                | (aBus << 16)
                // Bits 15-11
                | ((aSlot & 0x1F) << 11)
                // Bits 10-8
                | ((aFunction & 0x07) << 8));
        }

        #region Enums
        public enum PCIHeaderType : byte
        {
            Normal = 0x00,
            Bridge = 0x01,
            Cardbus = 0x02
        }

        [Flags]
        public enum PCIBist : byte
        {
            CocdMask = 0x0f,   /* Return result */
            Start = 0x40,   /* 1 to start BIST, 2 secs or less */
            Capable = 0x80    /* 1 if BIST capable */
        }

        public enum PCIInterruptPIN : byte
        {
            None = 0x00,
            INTA = 0x01,
            INTB = 0x02,
            INTC = 0x03,
            INTD = 0x04
        }
        #endregion
    }
}

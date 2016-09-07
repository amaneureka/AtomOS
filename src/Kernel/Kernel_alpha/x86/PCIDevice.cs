using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha.x86
{
    public class PCIDevice
    {
        #region Enums
        public enum PCIHeaderType : byte
        {
            Normal = 0x00,
            Bridge = 0x01,
            Cardbus = 0x02
        };

        public enum PCIBist : byte
        {
            CocdMask = 0x0f,   /* Return result */
            Start = 0x40,   /* 1 to start BIST, 2 secs or less */
            Capable = 0x80    /* 1 if BIST capable */
        };

        public enum PCIInterruptPIN : byte
        {
            None = 0x00,
            INTA = 0x01,
            INTB = 0x02,
            INTC = 0x03,
            INTD = 0x04
        };

        public enum Config : byte
        {
            VendorID            = 0,                            DeviceID            = 2,
            Command             = 4,                            Status              = 6,
            RevisionID          = 8,    ProgIF          = 9,    SubClass            = 10,   Class       = 11,
            CacheLineSize       = 12,   LatencyTimer    = 13,   HeaderType          = 14,   BIST        = 15,
            BAR0                = 16,
            BAR1                = 20,
            PrimaryBusNo        = 24,   SecondaryBusNo  = 25,   SubBusNo            = 26,   SecondarLT  = 27,
            IOBase              = 28,   IOLimit         = 29,   SecondaryStatus     = 30,
            MemoryBase          = 32,                           MemoryLimit         = 34,
            PrefMemoryBase      = 36,                           PrefMemoryLimit     = 38,
            PrefBase32Upper     = 40,
            PrefLimit32upper    = 44,
            PrefBase16Upper     = 48,                           PrefLimit16upper    = 50,
            CapabilityPointer   = 52,   Reserved        = 53,
            ExpROMBaseAddress   = 56,
            InterruptLine       = 60,   InterruptPIN    = 61,   BridgeControl       = 62
        };
        #endregion

        public readonly uint Bus;
        public readonly uint Slot;
        public readonly uint Function;

        public readonly ushort VendorID;
        public readonly ushort DeviceID;

        public readonly ushort Command;
        public readonly ushort Status;

        public readonly byte RevisionID;
        public readonly byte ProgIF;
        public readonly byte Subclass;
        public readonly byte ClassCode;
        public readonly byte SecondaryBusNumber;

        public readonly bool DeviceExists;

        public readonly PCIHeaderType HeaderType;
        public readonly PCIBist BIST;
        public readonly PCIInterruptPIN InterruptPIN;

        public const ushort ConfigAddressPort = 0xCF8;
        public const ushort ConfigDataPort = 0xCFC;

        public PCIBaseAddressBar[] BaseAddressBar;

        public PCIDevice(uint bus, uint slot, uint function)
        {
            this.Bus = bus;
            this.Slot = slot;
            this.Function = function;

            this.VendorID = ReadRegister16((byte)Config.VendorID);
            this.DeviceID = ReadRegister16((byte)Config.DeviceID);

            this.Command = ReadRegister16((byte)Config.Command);
            this.Status = ReadRegister16((byte)Config.Status);


            this.RevisionID = ReadRegister8((byte)Config.RevisionID);
            this.ProgIF = ReadRegister8((byte)Config.ProgIF);
            this.Subclass = ReadRegister8((byte)Config.SubClass);
            this.ClassCode = ReadRegister8((byte)Config.Class);
            this.SecondaryBusNumber = ReadRegister8((byte)Config.SecondaryBusNo);

            this.HeaderType = (PCIHeaderType)ReadRegister8((byte)Config.HeaderType);
            this.BIST = (PCIBist)ReadRegister8((byte)Config.BIST);
            this.InterruptPIN = (PCIInterruptPIN)ReadRegister8((byte)Config.InterruptPIN);

            DeviceExists = (uint)VendorID != 0xFFFF && (uint)DeviceID != 0xFFFF;
            if (HeaderType == PCIHeaderType.Normal)
            {
                BaseAddressBar = new PCIBaseAddressBar[6];
                BaseAddressBar[0] = new PCIBaseAddressBar(ReadRegister32(0x10));
                BaseAddressBar[1] = new PCIBaseAddressBar(ReadRegister32(0x14));
                BaseAddressBar[2] = new PCIBaseAddressBar(ReadRegister32(0x18));
                BaseAddressBar[3] = new PCIBaseAddressBar(ReadRegister32(0x1C));
                BaseAddressBar[4] = new PCIBaseAddressBar(ReadRegister32(0x20));
                BaseAddressBar[5] = new PCIBaseAddressBar(ReadRegister32(0x24));
            }
        }

        public static ushort GetHeaderType(ushort Bus, ushort Slot, ushort Function)
        {
            UInt32 xAddr = GetAddressBase(Bus, Slot, Function) | ((UInt32)(0xE & 0xFC));
            Native.Out32(ConfigAddressPort, xAddr);
            return (byte)(Native.In32(ConfigDataPort) >> ((0xE % 4) * 8) & 0xFF);
        }

        public static UInt16 GetVendorID(ushort Bus, ushort Slot, ushort Function)
        {
            UInt32 xAddr = GetAddressBase(Bus, Slot, Function) | ((UInt32)(0x0 & 0xFC));
            Native.Out32(ConfigAddressPort, xAddr);
            return (UInt16)(Native.In32(ConfigDataPort) >> ((0x0 % 4) * 8) & 0xFFFF); ;
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
        protected static UInt32 GetAddressBase(uint aBus, uint aSlot, uint aFunction)
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

        public void EnableMemory(bool enable)
        {
            UInt16 command = ReadRegister16(0x04);

            UInt16 flags = 0x0007;

            if (enable)
                command |= flags;
            else
                command &= (ushort)~flags;

            WriteRegister16(0x04, command);
        }
    }
    public class PCIBaseAddressBar
    {
        private uint baseAddress = 0;
        private ushort prefetchable = 0;
        private ushort type = 0;
        private bool isIO = false;

        public PCIBaseAddressBar(uint raw)
        {
            isIO = (raw & 0x01) == 1;

            if (isIO)
            {
                baseAddress = raw & 0xFFFFFFFC;
            }
            else
            {
                type = (ushort)((raw >> 1) & 0x03);
                prefetchable = (ushort)((raw >> 3) & 0x01);
                switch (type)
                {
                    case 0x00:
                        baseAddress = raw & 0xFFFFFFF0;
                        break;
                    case 0x01:
                        baseAddress = raw & 0xFFFFFFF0;
                        break;
                }
            }
        }

        public uint BaseAddress
        {
            get { return baseAddress; }
        }

        public bool IsIO
        {
            get { return isIO; }
        }
    }
}

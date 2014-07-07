using System;
using Kernel_alpha.x86.Intrinsic;
using Kernel_alpha.Drivers.ACPI;
using misc = Kernel_alpha.Drivers.ACPI.misc;

namespace Kernel_alpha.Drivers
{
    public unsafe class acpi
    {
        // New Port I/O
        private IOPort smiIO, pm1aIO, pm1bIO;

        // ACPI variables
        private int* SMI_CMD;
        private byte ACPI_ENABLE;
        private byte ACPI_DISABLE;
        private int* PM1a_CNT;
        private int* PM1b_CNT;
        private short SLP_TYPa;
        private short SLP_TYPb;
        private short SLP_EN;
        private short SCI_EN;
        private byte PM1_CNT_LEN;

        // FACP
        private byte* Facp = null;

        public acpi(bool initialize = true, bool enable = true)
        {
            if (initialize)
                Init ();

            if (enable)
                Enable ();
        }

        // Shutdown
        public void Shutdown ()
        {
            if (PM1a_CNT == null)
                Init ();

            pm1aIO.Word = (ushort)(SLP_TYPa | SLP_EN);

            if (PM1b_CNT != null)
                pm1bIO.Word = (ushort)(SLP_TYPb | SLP_EN);

            Native.Halt ();
        }

        // Reboot
        public void Reboot ()
        {
            IOPort port = new IOPort (0x64);
            byte good = 0x02;

            while ((good & 0x02) != 0)
                good = port.Byte;

            port.Byte = 0xFE;

            // Halt CPU
            Native.Halt ();
        }

        // Initializazion
        private bool Init()
        {
            byte* ptr = (byte*)RSDPAddress();
            int addr = 0;

            for (int i = 19; i >= 16; i--)
            {
                addr += (*((byte*)ptr + i));
                addr = (i == 16) ? addr : addr << 8;
            }

            ptr = (byte*)addr;
            ptr += 4; addr = 0;

            for (int i = 3; i >= 0; i--)
            {
                addr += (*((byte*)ptr + i));
                addr = (i == 0) ? addr : addr << 8;
            }

            int length = addr;
            ptr -= 4;

            if (ptr != null && misc.acpiCheckHeader((byte*)ptr, "RSDT") == 0)
            {
                addr = 0;
                int entrys = length;
                entrys = (entrys - 36) / 4;
                ptr += 36;
                byte* yeuse;

                while (0 < entrys--)
                {
                    for (int i = 3; i >= 0; i--)
                    {
                        addr += (*((byte*)ptr + i));
                        addr = (i == 0) ? addr : addr << 8;
                    }

                    yeuse = (byte*)addr;
                    Facp = (byte*)yeuse;

                    if (misc.Compare("FACP", Facp) == 0)
                    {
                        if (misc.acpiCheckHeader((byte*)facpget(0), "DSDT") == 0)
                        {
                            byte* S5Addr = (byte*)facpget(0) + 36;
                            int dsdtLength = *(facpget(0) + 1) - 36;

                            while (0 < dsdtLength--)
                            {
                                if (misc.Compare("_S5_", (byte*)S5Addr) == 0)
                                    break;
                                S5Addr++;
                            }

                            if (dsdtLength > 0)
                            {
                                if ((*(S5Addr - 1) == 0x08 || (*(S5Addr - 2) == 0x08 && *(S5Addr - 1) == '\\')) && *(S5Addr + 4) == 0x12)
                                {
                                    S5Addr += 5;
                                    S5Addr += ((*S5Addr & 0xC0) >> 6) + 2;
                                    if (*S5Addr == 0x0A)
                                        S5Addr++;
                                    SLP_TYPa = (short)(*(S5Addr) << 10);
                                    S5Addr++;
                                    if (*S5Addr == 0x0A)
                                        S5Addr++;
                                    SLP_TYPb = (short)(*(S5Addr) << 10);
                                    SMI_CMD = facpget(1);
                                    ACPI_ENABLE = facpbget(0);
                                    ACPI_DISABLE = facpbget(1);
                                    PM1a_CNT = facpget(2);
                                    PM1b_CNT = facpget(3);
                                    PM1_CNT_LEN = facpbget(3);
                                    SLP_EN = 1 << 13;
                                    SCI_EN = 1;

                                    smiIO = new IOPort((ushort)SMI_CMD);
                                    pm1aIO = new IOPort((ushort)PM1a_CNT);
                                    pm1bIO = new IOPort((ushort)PM1b_CNT);

                                    return true;
                                }
                            }
                        }
                    }

                    ptr += 4;
                }
            }

            return false;
        }

        // Enable ACPI
        private bool Enable ()
        {
            if (pm1aIO.Word == 0)
            {
                if (SMI_CMD != null && ACPI_ENABLE != 0)
                {
                    smiIO.Word = ACPI_ENABLE;

                    int i;
                    for (i = 0; i < 300; i++)
                    {
                        if ((pm1aIO.Word & 1) == 1)
                            break;
                    }

                    if (PM1b_CNT != null)
                    {
                        for (; i < 300; i++)
                        {
                            if ((pm1bIO.Word & 1) == 1)
                                break;
                        }
                    }

                    if (i < 300) return true;
                    else return false;
                }
                else return false;
            }
            else return true;
        }

        // Disable ACPI
        public void Disable ()
        {
            smiIO.Byte = ACPI_DISABLE;
        }

        // Retrieve the RSDP address
        private unsafe uint RSDPAddress()
        {
            for (uint addr = 0xE0000; addr < 0x100000; addr += 4)
                if (misc.Compare("RSD PTR ", (byte*)addr) == 0)
                    if (misc.Check_RSD(addr))
                        return addr;

            uint ebda_address = *((uint*)0x040E);
            ebda_address = (ebda_address * 0x10) & 0x000fffff;

            for (uint addr = ebda_address; addr < ebda_address + 1024; addr += 4)
                if (misc.Compare("RSD PTR ", (byte*)addr) == 0)
                    return addr;

            return 0;
        }

        // RSDT Table
        private uint* acpiCheckRSDPtr(uint* ptr)
        {
            string sig = "RSD PTR ";
            RSDPtr* rsdp = (RSDPtr*)ptr;
            
            byte* bptr;
            byte check = 0;
            int i;

            if (misc.Compare(sig, (byte*)rsdp) == 0)
            {
                bptr = (byte*)ptr;

                for (i = 0; i < 20; i++)
                {
                    check += *bptr;
                    bptr++;
                }

                if (check == 0)
                {
                    misc.Compare("RSDT", (byte*)rsdp->RsdtAddress);

                    if (rsdp->RsdtAddress != 0)
                        return (uint*)rsdp->RsdtAddress;
                }
            }

            return null;
        }

        // FACP Table
        private byte facpbget (int number)
        {
            if (number == 0) { return *(Facp + 52); }
            else if (number == 1) { return *(Facp + 53); }
            else if (number == 2) { return *(Facp + 89); }
            else return 0;
        }

        private int* facpget (int number)
        {
            if (number == 0) { return (int*)*((int*)(Facp + 40)); }
            else if (number == 1) { return (int*)*((int*)(Facp + 48)); }
            else if (number == 2) { return (int*)*((int*)(Facp + 64)); }
            else if (number == 3) { return (int*)*((int*)(Facp + 68)); }
            else { return null; }
        }
    }
}

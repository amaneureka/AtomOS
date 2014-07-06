using System;
using System.Runtime.InteropServices;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha.Drivers.HAL
{
    public unsafe class ACPI
    {
        // ACPI variables
        public int* SMI_CMD;
        public byte ACPI_ENABLE;
        public byte ACPI_DISABLE;
        public int* PM1a_CNT;
        public int* PM1b_CNT;
        public short SLP_TYPa;
        public short SLP_TYPb;
        public short SLP_EN;
        public short SCI_EN;
        public byte PM1_CNT_LEN;

        // Port I/O        
        public ushort smiIOPort, pm1aIOPort, pm1bIOPort;

        // ACPI structures
        [StructLayout (LayoutKind.Sequential, Pack = 1)]
        public struct RSDPtr
        {
            public fixed byte Signature[8];
            public byte CheckSum;
            public fixed byte OemID[6];
            public byte Revision;
            public int RsdtAddress;
        };

        [StructLayout (LayoutKind.Sequential, Pack = 1)]
        public struct FACP
        {
            public fixed byte Signature[4];
            public int Length;
            public fixed byte unneded1[40 - 8];
            public int* DSDT;
            public fixed byte unneded2[48 - 44];
            public int* SMI_CMD;
            public byte ACPI_ENABLE;
            public byte ACPI_DISABLE;
            public fixed byte unneded3[64 - 54];
            public int* PM1a_CNT_BLK;
            public int* PM1b_CNT_BLK;
            public fixed byte unneded4[89 - 72];
            public byte PM1_CNT_LEN;
        };

        // Initialize ACPI
        public bool Init ()
        {
            byte* ptr = (byte*)RSDPAddress ();
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

            if (ptr != null && acpiCheckHeader ((byte*)ptr, "RSDT") == 0)
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

                    if (Compare ("FACP", Facp) == 0)
                    {
                        if (acpiCheckHeader ((byte*)facpget (0), "DSDT") == 0)
                        {
                            byte* S5Addr = (byte*)facpget (0) + 36;
                            int dsdtLength = *(facpget (0) + 1) - 36;

                            while (0 < dsdtLength--)
                            {
                                if (Compare ("_S5_", (byte*)S5Addr) == 0)
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
                                    SMI_CMD = facpget (1);
                                    ACPI_ENABLE = facpbget (0);
                                    ACPI_DISABLE = facpbget (1);
                                    PM1a_CNT = facpget (2);
                                    PM1b_CNT = facpget (3);
                                    PM1_CNT_LEN = facpbget (3);
                                    SLP_EN = 1 << 13;
                                    SCI_EN = 1;
                                    smiIOPort = (ushort)SMI_CMD;
                                    pm1aIOPort = (ushort)PM1a_CNT;
                                    pm1bIOPort = (ushort)PM1b_CNT;
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
        public bool Enable ()
        {
            if (Native.In16 (pm1aIOPort) == 0)
            {
                if (SMI_CMD != null && ACPI_ENABLE != 0)
                {
                    Native.Out8(smiIOPort, ACPI_ENABLE);
                    int i;
                    for (i = 0; i < 300; i++)
                    {
                        if ((Native.In16 (pm1aIOPort) & 1) == 1)
                            break;
                    }
                    if (PM1b_CNT != null)
                        for (; i < 300; i++)
                        {
                            if ((Native.In16 (pm1bIOPort) & 1) == 1)
                                break;
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
            Native.Out8(smiIOPort, ACPI_DISABLE);
        }

        // Comparation
        public int Compare (string c1, byte* c2)
        {
            for (int i = 0; i < c1.Length; i++)
            {
                if (c1[i] != (char)c2[i]) { return -1; }
            }
            return 0;
        }

        public int acpiCheckHeader (byte* ptr, string sig)
        {
            return Compare (sig, ptr);
        }

        // FACP
        public byte* Facp = null;

        public byte facpbget (int number)
        {
            if (number == 0) { return *(Facp + 52); }
            else if (number == 1) { return *(Facp + 53); }
            else if (number == 2) { return *(Facp + 89); }
            else return 0;
        }

        public int* facpget (int number)
        {
            if (number == 0) { return (int*)*((int*)(Facp + 40)); }
            else if (number == 1) { return (int*)*((int*)(Facp + 48)); }
            else if (number == 2) { return (int*)*((int*)(Facp + 64)); }
            else if (number == 3) { return (int*)*((int*)(Facp + 68)); }
            else { return null; }
        }

        // RSD
        public bool Check_RSD (uint address)
        {
            byte sum = 0;
            byte* check = (byte*)address;

            for (int i = 0; i < 20; i++)
                sum += *(check++);

            return (sum == 0);
        }

        public uint* acpiCheckRSDPtr (uint* ptr)
        {
            string sig = "RSD PTR ";
            RSDPtr* rsdp = (RSDPtr*)ptr;

            byte* bptr;
            byte check = 0;
            int i;

            if (Compare (sig, (byte*)rsdp) == 0)
            {
                bptr = (byte*)ptr;

                for (i = 0; i < 20; i++)
                {
                    check += *bptr;
                    bptr++;
                }

                if (check == 0)
                {
                    Compare ("RSDT", (byte*)rsdp->RsdtAddress);

                    if (rsdp->RsdtAddress != 0)
                        return (uint*)rsdp->RsdtAddress;
                }
            }

            return null;
        }

        public unsafe uint RSDPAddress ()
        {
            for (uint addr = 0xE0000; addr < 0x100000; addr += 4)
                if (Compare ("RSD PTR ", (byte*)addr) == 0)
                    if (Check_RSD (addr))
                        return addr;

            uint ebda_address = *((uint*)0x040E);
            ebda_address = (ebda_address * 0x10) & 0x000fffff;

            for (uint addr = ebda_address; addr < ebda_address + 1024; addr += 4)
                if (Compare ("RSD PTR ", (byte*)addr) == 0)
                    return addr;

            return 0;
        }

        // Shutdown
        public void Shutdown ()
        {
            if (PM1a_CNT == null)
                Init ();

            Native.Out16(pm1aIOPort, (ushort)(SLP_TYPa | SLP_EN));
            
            if (PM1b_CNT != null)
                Native.Out16(pm1bIOPort, (ushort)(SLP_TYPb | SLP_EN));

            // Halt CPU
            Native.Halt();
        }

        // Reboot
        public void Reboot ()
        {
            byte good = 0x02;

            while ((good & 0x02) != 0)
                good = Native.In8 (0x64);

            Native.Out8 (0x64, 0xFE);

            // Halt CPU
            Native.Halt();
        }
    }
}

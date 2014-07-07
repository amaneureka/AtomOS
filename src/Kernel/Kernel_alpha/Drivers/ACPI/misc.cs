using Kernel_alpha.x86.Intrinsic;
using System;
using System.Runtime.InteropServices;

namespace Kernel_alpha.Drivers.ACPI
{
    public unsafe static class misc
    {
        public static int Compare (string c1, byte* c2)
        {
            for (int i = 0; i < c1.Length; i++)
            {
                if (c1[i] != (char)c2[i])
                    return -1;
            }
            return 0;
        }

        public static int acpiCheckHeader (byte* ptr, string sig)
        {
            return Compare (sig, ptr);
        }

        // RSD
        public static bool Check_RSD (uint address)
        {
            byte sum = 0;
            byte* check = (byte*)address;

            for (int i = 0; i < 20; i++)
                sum += *(check++);

            return (sum == 0);
        }        
    }

    /// <summary>
    /// RSD Table
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public unsafe struct RSDPtr
    {
        public fixed byte Signature[8];
        public byte CheckSum;
        public fixed byte OemID[6];
        public byte Revision;
        public int RsdtAddress;
    };

    /// <summary>
    /// FACP Table
    /// </summary>
    [StructLayout (LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FACP
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
}

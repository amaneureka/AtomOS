using System;
using Kernel_alpha.x86.Intrinsic;

using misc = Kernel_alpha.Drivers.ACPI.misc;

namespace Kernel_alpha.Drivers
{
    public unsafe class acpi
    {
        public acpi(bool initialize = true, bool enable = true)
        {
            if (initialize)
                misc.Init ();

            if (enable)
                Enable ();
        }

        // Shutdown
        public void Shutdown ()
        {
            if (misc.PM1a_CNT == null)
                misc.Init ();

            misc.pm1aIO.Word = (ushort)(misc.SLP_TYPa | misc.SLP_EN);

            if (misc.PM1b_CNT != null)
                misc.pm1bIO.Word = (ushort)(misc.SLP_TYPb | misc.SLP_EN);

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

        // Enable ACPI
        public bool Enable ()
        {
            if (misc.pm1aIO.Word == 0)
            {
                if (misc.SMI_CMD != null && misc.ACPI_ENABLE != 0)
                {
                    misc.smiIO.Word = misc.ACPI_ENABLE;

                    int i;
                    for (i = 0; i < 300; i++)
                    {
                        if ((misc.pm1aIO.Word & 1) == 1)
                            break;
                    }

                    if (misc.PM1b_CNT != null)
                    {
                        for (; i < 300; i++)
                        {
                            if ((misc.pm1bIO.Word & 1) == 1)
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
            misc.smiIO.Byte = misc.ACPI_DISABLE;
        }
    }
}

using System;
using Kernel_alpha.x86;
using Kernel_alpha.x86.Intrinsic;
using System.Collections.Generic;
using Kernel_alpha.Drivers;
using Kernel_alpha.Drivers.Input;
using Kernel_alpha.Drivers.Buses.ATA;

namespace Kernel_alpha
{
    public static class Global
    {
        public static Keyboard KBD;
        public static PS2Mouse Mouse;
        public static acpi ACPI;

        public static IDE PrimaryIDE;
        public static IDE SecondayIDE;

        public static List<BlockDevice> Devices = new List<BlockDevice>();

        public static void Init()
        {
            //Load Serial Port at COM-1
            Console.Write("Loading Serial Ports... ");
            Serials.SetupPort();
            Console.WriteLine("OK");

            // Setup PCI
            Console.Write ("Setting up PCI... ");
            PCI.Setup();
            Console.WriteLine ("OK");

            // Start ACPI
            // Initializes and enables itself
            Console.Write ("Setting up ACPI... ");
            //ACPI = new acpi(true, true);
            Console.WriteLine ("OK");

            // Setup Keyboard
            Console.Write("Setting up PS/2 Keyboard... ");
            KBD = new Keyboard();
            Console.WriteLine("OK");

            // Setup Mouse
            //Console.Write ("Setting up PS/2 Mouse... ");
            Mouse = new PS2Mouse();
            //Console.WriteLine ("OK");

            //Loading ATA
            Console.Write ("Loading ATA/SATA SubSystem... ");
            LoadATA();
            Console.WriteLine("OK");

            //Load Parts
            Console.Write ("Loading Partitions... ");
            if (PrimaryIDE != null && PrimaryIDE.DriveInfo.Device == Device.IDE_ATA)
            {
                var xMBR = new Drivers.PartInfo.MBR(PrimaryIDE);
                for (int i = 0; i < xMBR.PartInfo.Count; i++)
                    Devices.Add(xMBR.PartInfo[i]);
            }
            Console.WriteLine("OK");
        }

        private static void LoadATA()
        {
            PCIDevice xDevice = PCI.GetDeviceClass(0x1, 0x1); //Media Storage - IDE Controller
            if (xDevice != null)
            {
                //We are going to support only parallel ata
                PrimaryIDE = new IDE(false);
                SecondayIDE = new IDE(true);

                if (PrimaryIDE.DriveInfo.Device != Device.IDE_None)
                {
                    Devices.Add(PrimaryIDE);
                    xINT.RegisterHandler(delegate() { PrimaryIDE.IRQInvoked = true; }, 0x2E);
                }

                if (SecondayIDE.DriveInfo.Device != Device.IDE_None)
                {
                    Devices.Add(SecondayIDE);
                    xINT.RegisterHandler(delegate() { SecondayIDE.IRQInvoked = true; }, 0x2F);
                }
            }
            /*
            xDevice = PCI.GetDeviceClass(0x1, 0x6);//Media Storage - SATA
            if (xDevice != null)
            {

            }*/
        }
    }
}

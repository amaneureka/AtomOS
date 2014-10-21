using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.x86
{
    public static class PCI
    {
        private static List<PCIDevice> Devices;

        public static uint Count
        {
            get { return (uint)Devices.Count; }
        }

        public static void Setup()
        {
            Devices = new List<PCIDevice>();

            //EnumerateBus(0x0);
            if ((PCIDevice.GetHeaderType(0x0, 0x0, 0x0) & 0x80) == 0)
            {
                /* Single PCI host controller */
                CheckBus(0x0);
            }
            else
            {
                /* Multiple PCI host controllers */
                for (ushort fn = 0; fn < 8; fn++)
                {
                    if (PCIDevice.GetVendorID(0x0, 0x0, fn) != 0xFFFF)
                        break;

                    CheckBus(fn);
                }
            }
        }

        private static void CheckBus(ushort xBus)
        {
            for (ushort device = 0; device < 32; device++)
            {
                if (PCIDevice.GetVendorID(xBus, device, 0x0) == 0xFFFF)
                    continue;

                CheckFunction(new PCIDevice(xBus, device, 0x0));
                if ((PCIDevice.GetHeaderType(xBus, device, 0x0) & 0x80) != 0)
                {
                    /* It is a multi-function device, so check remaining functions */
                    for (ushort fn = 1; fn < 8; fn++)
                    {
                        if (PCIDevice.GetVendorID(xBus, device, fn) != 0xFFFF)
                            CheckFunction(new PCIDevice(xBus, device, fn));
                    }
                }
            }
        }

        private static void CheckFunction(PCIDevice xPCIDevice)
        {
            Devices.Add(xPCIDevice);

            if (xPCIDevice.ClassCode == 0x6 && xPCIDevice.Subclass == 0x4)
                CheckBus(xPCIDevice.SecondaryBusNumber);
        }

        public static PCIDevice GetDeviceVendorID(ushort VendorID, ushort DeviceID)
        {
            for (int i = 0; i < Devices.Count; i++)
            {
                var xDevice = Devices[i];
                if (xDevice.VendorID == VendorID && xDevice.DeviceID == DeviceID)
                {
                    return Devices[i];
                }
            }
            return null;
        }

        public static PCIDevice GetDeviceClass(ushort Class, ushort SubClass)
        {
            for (int i = 0; i < Devices.Count; i++)
            {
                var xDevice = Devices[i];
                if (xDevice.ClassCode == Class && xDevice.Subclass == SubClass)
                {
                    return Devices[i];
                }
            }
            return null;
        }
    }
}

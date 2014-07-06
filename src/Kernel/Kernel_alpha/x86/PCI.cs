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
            EnumerateBus(0x0, 0x0);
        }

        private static void EnumerateBus(ushort xBus, ushort step)
        {
            PCIDevice xPCIDevice;

            for (uint i = 0; i < 32; i++)
            {
                xPCIDevice = new PCIDevice(xBus, i, 0x0);
                if (xPCIDevice.DeviceExists)
                {
                    if (xPCIDevice.HeaderType == PCIDevice.PCIHeaderType.Bridge)
                    {
                        //Have to do work here
                    }
                    else
                    {                        
                        Devices.Add(xPCIDevice);
                    }
                }
            }
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

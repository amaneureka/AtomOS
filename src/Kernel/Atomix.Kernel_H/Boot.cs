using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.gui;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.devices;
using Atomix.Kernel_H.io.FileSystem;

using Atomix.Kernel_H.drivers.input;
using Atomix.Kernel_H.drivers.buses.ATA;

namespace Atomix.Kernel_H
{
    public class Boot
    {
        public static void Init()
        {
            Debug.Write("Boot Init()\n");

            Keyboard.Setup();
            Mouse.Setup();

            #region IDE Devices
            /* Poll through all possible IDE Devices and mount them If they exist */
            LoadIDE(true, true);
            LoadIDE(false, true);
            #endregion

            Compositor.Setup(Scheduler.SystemProcess);

            var packet = new byte[32];

            packet.SetUInt(0, 0xDEADCAFE);
            packet.SetByte(4, 0xCC);
            packet.SetInt(9, 512);
            packet.SetInt(13, 512);
            Compositor.SERVER.Write(packet);
        }

        public static void LoadIDE(bool IsPrimary, bool IsMaster)
        {
            var xIDE = new IDE(IsPrimary, IsMaster);

            bool Clean = true;
            if (xIDE.IsValid)
            {
                switch(xIDE.Device)
                {
                    case Device.IDE_ATA:
                        {
                            /*
                             * First we check If it has partitions,
                             *      If parition.count > 0
                             *          Add Individual Partitions
                             */
                            var xMBR = new MBR(xIDE);
                            if (xMBR.PartInfo.Count > 0)
                            {
                                for (int i = 0; i < xMBR.PartInfo.Count; i++)
                                {
                                    /*
                                     * Iterate over all FileSystem Drivers and check which is valid
                                     */
                                    var xFileSystem = new FatFileSystem(xMBR.PartInfo[i]);
                                    if (xFileSystem.IsValid)
                                    {
                                        VirtualFileSystem.MountDevice(null, xFileSystem);
                                        Clean = false;
                                    }
                                    else
                                        Heap.Free(xFileSystem);
                                }
                            }
                            xMBR.Clean();
                        }
                        break;
                }
            }

            if (Clean)
                Heap.Free(xIDE);
        }
    }
}

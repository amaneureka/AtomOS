/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Boot Extension Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Lib;

using Atomix.Kernel_H.IO;
using Atomix.Kernel_H.Gui;
using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Devices;
using Atomix.Kernel_H.Arch.x86;
using Atomix.Kernel_H.Drivers.Video;
using Atomix.Kernel_H.IO.FileSystem;

using Atomix.Kernel_H.Drivers.Input;
using Atomix.Kernel_H.Drivers.buses.ATA;

namespace Atomix.Kernel_H
{
    internal class Boot
    {
        internal static int ClientID;
        internal static Pipe SystemClient;

        internal static void Init()
        {
            Debug.Write("Boot Init()\n");

            #region InitRamDisk
            if (Multiboot.RamDisk != 0)
            {
                var xFileSystem = new RamFileSystem(Multiboot.RamDisk, Multiboot.RamDiskSize);
                if (xFileSystem.IsValid)
                    VirtualFileSystem.MountDevice(null, xFileSystem);
                else
                    throw new Exception("RamDisk Corrupted!");
            }
            else
                throw new Exception("RamDisk not found!");
            #endregion
            #region PS2 Devices
            Keyboard.Setup();
            Mouse.Setup();
            #endregion
            #region Compositor
            SystemClient = new Pipe(Compositor.PACKET_SIZE, 100);
            Compositor.Setup(Scheduler.SystemProcess);
            ClientID = Compositor.CreateConnection(SystemClient);

            new Thread(Scheduler.SystemProcess, BootAnimation, Heap.kmalloc(0x1000) + 0x1000, 0x1000).Start();
            #endregion
            #region IDE Devices
            LoadIDE(true, true);
            LoadIDE(false, true);
            #endregion

            // pp = IntPtr.Add(IntPtr.Zero, 100);
            //Debug.Write("Test1234: %d\n", (uint)pp.ToInt32());

            //FILE READING TEST
            /*var stream = VirtualFileSystem.GetFile("disk0/gohu-11.bdf");
            if (stream != null)
                Debug.Write(stream.ReadToEnd());
            else
                Debug.Write("File not found!\n");
            Debug.Write("lol: %d\n", __main(0xac));*/
            while (true) ;
        }

        internal static unsafe void BootAnimation()
        {
            VBE.Clear(0x6D6D6D);
            var BootImage = VirtualFileSystem.GetFile("disk0/boot.xmp");
            if (BootImage != null)
            {
                var xData = new byte[Compositor.PACKET_SIZE];
                var Request = (GuiRequest*)Native.GetContentAddress(xData);
                Request->ClientID = ClientID;
                Request->Type = RequestType.NewWindow;

                var Request2 = (NewWindow*)Request;
                Request2->X = 512;
                Request2->Y = 150;
                Request2->Width = 256;
                Request2->Height = 256;
                Compositor.Server.Write(xData);
                SystemClient.Read(xData);

                string HashCode = new string(Request2->Hash);
                Debug.Write("WinHash: %s\n", HashCode);
                var aBuffer = SHM.Obtain(HashCode, 0, false);

                BootImage.Read(xData, 8);
                uint c = 0;
                uint index = 0;
                while ((c = (uint)BootImage.Read(xData, 32)) > 0)
                {
                    Memory.FastCopy(aBuffer + index, (uint)Request, c);
                    index += c;
                }
                Heap.Free(xData);
                Debug.Write("Ticks: %d\n", Timer.TicksFromStart);
            }
            else
                Debug.Write("Boot Image not found!\n");
        }

        internal static void LoadIDE(bool IsPrimary, bool IsMaster)
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

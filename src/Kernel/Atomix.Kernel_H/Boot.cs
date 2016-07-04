/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Boot Extension Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.IO;
using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.Gui;
using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.devices;
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
            SystemClient = new Pipe(32, 100);
            Compositor.Setup(Scheduler.SystemProcess);
            ClientID = Compositor.AddClient(SystemClient);

            var xTempStack = Heap.kmalloc(0x1000);
            new Thread(Scheduler.SystemProcess, pBootAnimation, xTempStack + 0x1000, 0x1000).Start();
            #endregion
            #region IDE Devices
            LoadIDE(true, true);
            LoadIDE(false, true);
            #endregion

            //FILE READING TEST
            /*var stream = VirtualFileSystem.GetFile("disk0/gohu-11.bdf");
            if (stream != null)
                Debug.Write(stream.ReadToEnd());
            else
                Debug.Write("File not found!\n");
            Debug.Write("lol: %d\n", __main(0xac));*/
            while (true) ;
        }

        private static uint pBootAnimation;
        internal static unsafe void BootAnimation()
        {
            VBE.Clear(0x6D6D6D);
            var BootImage = VirtualFileSystem.GetFile("disk0/boot.xmp");
            if (BootImage != null)
            {
                var Request = Compositor.RequestPacket(ClientID);
                Request.SetByte(4, 0xCC);
                Request.SetInt(9, 256);
                Request.SetInt(13, 256);
                Request.SetInt(17, 512);
                Request.SetInt(21, 150);
                Compositor.SERVER.Write(Request);
                SystemClient.Read(Request);

                string HashCode = Lib.encoding.ASCII.GetString(Request, 13, 19);
                var aBuffer = (byte*)SHM.Obtain(HashCode, -1);
                
                BootImage.Read(Request, 8);
                int c = 0;
                while((c = BootImage.Read(Request, 32)) != 0)
                {
                    for (int i = 0; i < c; i++, aBuffer++)
                        *aBuffer = Request[i];
                }

                Heap.Free(Request);
                Request = Compositor.RequestPacket(ClientID);
                Request.SetByte(4, 0xDA);
                Request.SetStringASCII(9, HashCode);
                Request.SetByte((uint)(HashCode.Length + 9), 0x0);
                Compositor.SERVER.Write(Request);

                Heap.Free(Request);
            }
            else
                Debug.Write("Boot Image not found!\n");
            Thread.Die();
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

using System;

using Atomix.Kernel_H.io;
using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.gui;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.devices;
using Atomix.Kernel_H.arch.x86;
using Atomix.Kernel_H.drivers.video;
using Atomix.Kernel_H.io.FileSystem;

using Atomix.Kernel_H.drivers.input;
using Atomix.Kernel_H.drivers.buses.ATA;

namespace Atomix.Kernel_H
{
    public class Boot
    {
        public static int ClientID;
        public static Pipe SystemClient;

        public static void Init()
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
            Debug.Write("Reading Test\n");
            var stream = VirtualFileSystem.GetFile("disk1/README.TXT");
            if (stream != null)
            {
                var xData = new byte[256];
                int c = 0;
                while ((c = stream.Read(xData, xData.Length)) != 0)
                    Debug.Write(lib.encoding.ASCII.GetString(xData, 0, c));
            }
            else
                Debug.Write("File not found!\n");
            while (true) ;
        }
        
        public static uint pBootAnimation;
        public static unsafe void BootAnimation()
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

                string HashCode = lib.encoding.ASCII.GetString(Request, 13, 19);
                var aBuffer = (byte*)SHM.Obtain(HashCode, 0, false);
                
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

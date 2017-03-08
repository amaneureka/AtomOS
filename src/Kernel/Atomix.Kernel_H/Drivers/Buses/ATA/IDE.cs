/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          IDE Driver
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Devices;
using Atomix.Kernel_H.Arch.x86;

namespace Atomix.Kernel_H.Drivers.buses.ATA
{
    internal unsafe class IDE : Storage
    {
        protected UInt16 DataReg;
        protected UInt16 FeatureReg;
        protected UInt16 SectorCountReg;
        protected UInt16 CommandReg;
        protected UInt16 StatusReg;
        protected UInt16 AltStatusReg;
        protected UInt16 ControlReg;

        protected UInt16 LBA0;
        protected UInt16 LBA1;
        protected UInt16 LBA2;

        protected UInt16 DeviceSelect;

        protected Channel mChannel;
        protected Device mDevice;
        protected Type mType;

        protected UInt32 mCylinder;
        protected UInt32 mHeads;
        protected UInt32 mSize;
        protected UInt32 mSectorsPerTrack;
        protected UInt32 mCommandSet;
        protected string mModel;
        protected string mSerialNo;
        protected bool mLBASupport;
        protected bool mIsRemovable;
        protected int mBufferSize;
        protected byte[] mATAPI_Packet;

        protected bool IRQInvoked;

        internal IDE(bool IsPrimary, bool IsMaster = true)
        {
            UInt16 xBAR0 = (UInt16)(IsPrimary ? 0x01F0 : 0x0170);
            UInt16 xBAR1 = (UInt16)(IsPrimary ? 0x03F6 : 0x0376);

            mChannel = IsPrimary ? Channel.PRIMARY : Channel.SECONDARY;
            mType = IsMaster ? Type.MASTER : Type.SLAVE;

            DataReg         = (UInt16)(xBAR0 + (byte)Register.ATA_REG_DATA);
            FeatureReg      = (UInt16)(xBAR0 + (byte)Register.ATA_REG_FEATURES);
            SectorCountReg  = (UInt16)(xBAR0 + (byte)Register.ATA_REG_SECCOUNT0);
            CommandReg      = (UInt16)(xBAR0 + (byte)Register.ATA_REG_COMMAND);
            StatusReg       = (UInt16)(xBAR0 + (byte)Register.ATA_REG_STATUS);
            AltStatusReg    = (UInt16)(xBAR1 + (byte)Register.ATA_REG_ALTSTATUS);
            ControlReg      = (UInt16)(xBAR1 + (byte)Register.ATA_REG_CONTROL);

            LBA0 = (UInt16)(xBAR0 + (byte)Register.ATA_REG_LBA0);
            LBA1 = (UInt16)(xBAR0 + (byte)Register.ATA_REG_LBA1);
            LBA2 = (UInt16)(xBAR0 + (byte)Register.ATA_REG_LBA2);

            DeviceSelect = (UInt16)(xBAR0 + (byte)Register.ATA_REG_HDDEVSEL);

            // Disable IRQ
            PortIO.Out8(ControlReg, 0x2);

            // Discover what we have =P
            Discover();

            if (mDevice != Device.IDE_None)
            {
                IRQInvoked = false;

                // Register Interrupt Handler :-)
                IDT.RegisterInterrupt(
                    delegate(ref IRQContext xContext)
                    {
                        IRQInvoked = true;
                    },
                    (uint)(IsPrimary ? 0x2E : 0x2F));
            }
        }

        internal Device Device { get { return mDevice; } }

        internal bool IsValid
        {
            get { return (mDevice != Device.IDE_None); }
        }

        /// <summary>
        /// This method discover the current ATA device and try to read all its configurations
        /// </summary>
        private void Discover()
        {
            mDevice = Device.IDE_None;
            mBufferSize = 0;

            Status xStatus;
            bool Error = false;

            // Select Drive
            SelectDrive();

            // Send Identify command
            PortIO.Out8(CommandReg, (byte)Cmd.ATA_CMD_IDENTIFY);
            Wait();

            if (PortIO.In8(StatusReg) == 0)
                return; // No Device

            while (true)
            {
                xStatus = (Status)PortIO.In8(StatusReg);
                if ((xStatus & Status.ATA_SR_ERR) != 0)
                {
                    Error = true; // If Err, Device is not ATA.
                    break;
                }

                if (((xStatus & Status.ATA_SR_BSY) == 0) && ((xStatus & Status.ATA_SR_DRQ) != 0))
                    break; // Everything is fine
                Wait();
            }

            mDevice = Device.IDE_ATA;
            mBufferSize = 512;

            // (IV) Probe for ATAPI Devices:
            if (Error)
            {
                ushort xTypeID = (ushort)(PortIO.In8(LBA2) << 8 | PortIO.In8(LBA1));
                if (xTypeID == 0xEB14 || xTypeID == 0x9669)
                {
                    mDevice = Device.IDE_ATAPI;
                    mBufferSize = 2048;
                    mATAPI_Packet = new byte[12];
                }
                else
                {
                    mDevice = Device.IDE_None;
                    mBufferSize = 0;
                    return;
                }

                // Send Identify packet command
                PortIO.Out8(CommandReg, (byte)Cmd.ATA_CMD_IDENTIFY_PACKET);
                Wait();
            }

            var xBuff = new ushort[256];
            PortIO.Read16(DataReg, xBuff);

            // ATA/ATAPI COnfig
            mIsRemovable        = (xBuff[(int)Identify.ATA_IDENT_DEVICETYPE] & (1 << 7)) > 0;

            // CHS configurations
            mCylinder           = xBuff.ToUInt32((int)Identify.ATA_IDENT_CYLINDERS);
            mHeads              = xBuff.ToUInt32((int)Identify.ATA_IDENT_HEADS);
            mSectorsPerTrack    = xBuff.ToUInt32((int)Identify.ATA_IDENT_SECTORS);
            mCommandSet         = xBuff.ToUInt32((int)Identify.ATA_IDENT_COMMANDSETS);

            ushort xFieldValid  = xBuff[(int)Identify.ATA_IDENT_FIELDVALID];
            // 1st bit determine weather it support LBA or not
            mLBASupport         = (bool)((xFieldValid & 1) == 1);

            if ((mCommandSet & (1 << 26)) != 0)
                // Device uses 48-Bit Addressing:
                throw new Exception("48bit addresssing not supported");
            //mSize = xBuff.ToUInt48((int)Identify.ATA_IDENT_MAX_LBA_EXT);
            else
                // Device uses CHS or 28-bit Addressing:
                mSize = xBuff.ToUInt32((int)Identify.ATA_IDENT_MAX_LBA);

            // Read Model, Firmware, SerialNo.
            mModel      = xBuff.GetString((int)Identify.ATA_IDENT_MODEL, 40);
            mSerialNo   = xBuff.GetString((int)Identify.ATA_IDENT_SERIAL, 20);

            Heap.Free(xBuff);
        }

        internal override bool Read(uint SectorNo, uint SectorCount, byte[] xData)
        {
            return false;
        }

        internal override unsafe bool Read(uint SectorNo, uint SectorCount, byte* xData)
        {
            return false;
        }

        internal override bool Write(uint SectorNo, uint SectorCount, byte[] xData)
        {
            return false;
        }

        internal override unsafe bool Write(uint SectorNo, uint SectorCount, byte* xData)
        {
            return false;
        }

        /*
        private bool Access_Disk(UInt32 SectorNo, uint SectorCount, byte* xData, bool IsReading)
        {
            if (mDevice == Device.IDE_ATAPI)
            {
                if (IsReading)
                {
                    if (SectorCount != 1)// Only 1 sector we can read at a time
                        return false;

                    // Lock up device
                    //Monitor.AcquireLock(this);

                    // SCSI Packet Command
                    mATAPI_Packet[0] = (byte)Cmd.ATAPI_CMD_READ;
                    mATAPI_Packet[1] = 0x00;
                    mATAPI_Packet[2] = (byte)((SectorNo >> 24) & 0xFF);
                    mATAPI_Packet[3] = (byte)((SectorNo >> 16) & 0xFF);
                    mATAPI_Packet[4] = (byte)((SectorNo >> 8) & 0xFF);
                    mATAPI_Packet[5] = (byte)((SectorNo >> 0) & 0xFF);
                    mATAPI_Packet[6] = 0x00;
                    mATAPI_Packet[7] = 0x00;
                    mATAPI_Packet[8] = 0x00;
                    mATAPI_Packet[9] = (byte)(SectorCount & 0xFF);
                    mATAPI_Packet[10] = 0x00;
                    mATAPI_Packet[11] = 0x00;

                    // Enable IRQ
                    IRQInvoked = false;
                    PortIO.Out8(ControlReg, 0x0);

                    SelectDrive();

                    PortIO.Out8(FeatureReg, 0x0);// Tell controller that we are going to use PIO mode

                    // Tell constroller the size of each buffer
                    PortIO.Out8(LBA1, (byte)((mBufferSize) & 0xFF));// Lower Byte of Sector Size. ATA_LBA_MID_PORT
                    PortIO.Out8(LBA2, (byte)((mBufferSize >> 8) & 0xFF));// Upper Byte of Sector Size. ATA_LBA_HI_PORT

                    // Send Packet command
                    Send_SCSI_Package();

                    // Actual size that is to transferred
                    int size = (PortIO.In8(LBA2) << 8 | PortIO.In8(LBA1));

                    // Read the data
                    PortIO.Read16(DataReg, xData, size);

                    WaitIRQ();
                    while (((Status)PortIO.In8(StatusReg) & (Status.ATA_SR_BSY | Status.ATA_SR_DRQ)) != 0) ;

                    // UnLock up device
                    //Monitor.ReleaseLock(this);

                    return true;
                }
                return false;
            }
            else if (mDevice == Device.IDE_ATA)
            {
                // Lock up device
                //Monitor.AcquireLock(this);

                // Disable IRQ
                IRQInvoked = false;
                PortIO.Out8(ControlReg, 0x2);

                ushort lba_mode, cyl;
                byte head, sect;
                byte[] lba_io = new byte[6];

                // (I) Select one from LBA28, LBA48 or CHS;
                if (SectorNo >= 0x10000000)
                {
                    // LBA48:
                    lba_mode    = 2;
                    lba_io[0]   = (byte)((SectorNo & 0x000000FF) >> 0);
                    lba_io[1]   = (byte)((SectorNo & 0x0000FF00) >> 8);
                    lba_io[2]   = (byte)((SectorNo & 0x00FF0000) >> 16);
                    lba_io[3]   = (byte)((SectorNo & 0xFF000000) >> 24);
                    lba_io[4]   = 0; // LBA28 is integer, so 32-bits are enough to access 2TB.
                    lba_io[5]   = 0; // LBA28 is integer, so 32-bits are enough to access 2TB.
                    head        = 0; // Lower 4-bits of HDDEVSEL are not used here.
                }
                else if ((mCommandSet & (1 << 26)) != 0)
                {
                    // LBA28:
                    lba_mode    = 1;
                    lba_io[0]   = (byte)((SectorNo & 0x00000FF) >> 0);
                    lba_io[1]   = (byte)((SectorNo & 0x000FF00) >> 8);
                    lba_io[2]   = (byte)((SectorNo & 0x0FF0000) >> 16);
                    lba_io[3]   = 0; // These Registers are not used here.
                    lba_io[4]   = 0; // These Registers are not used here.
                    lba_io[5]   = 0; // These Registers are not used here.
                    head        = (byte)((SectorNo & 0xF000000) >> 24);
                }
                else
                {
                    // CHS:
                    lba_mode    = 0;
                    sect        = (byte)((SectorNo % 63) + 1);
                    cyl         = (ushort)((SectorNo + 1 - sect) / (16 * 63));
                    lba_io[0]   = (byte)(sect & 0xFF);
                    lba_io[1]   = (byte)((cyl >> 0) & 0xFF);
                    lba_io[2]   = (byte)((cyl >> 8) & 0xFF);
                    lba_io[3]   = 0;
                    lba_io[4]   = 0;
                    lba_io[5]   = 0;
                    head = (byte)((SectorNo + 1 - sect) % (16 * 63) / (63)); // Head number is written to HDDEVSEL lower 4-bits.
                }

                while (((Status)PortIO.In8(StatusReg) & Status.ATA_SR_BSY) != 0) ;

                // (IV) Select Drive from the controller;
                if (lba_mode == 0)
                    SelectDrive(head, false);
                else
                    SelectDrive(head, true);

                // (V) Write Parameters;
                if (lba_mode == 2)
                {
                    Debug.Write("IDE: LBA_MODE=2 YET TO IMPLEMENT\n");
                    throw new Exception("Yet to implement");
                }
                PortIO.Out8(SectorCountReg, (byte)(SectorCount & 0xFF));
                PortIO.Out8(LBA0, lba_io[0]);
                PortIO.Out8(LBA1, lba_io[1]);
                PortIO.Out8(LBA2, lba_io[2]);

                // Free up this memory
                Heap.Free(lba_io);

                // We are not using DMA so don't care about that
                byte cmd = 0;
                if (lba_mode == 0 && IsReading) cmd = (byte)Cmd.ATA_CMD_READ_PIO;
                else if (lba_mode == 1 && IsReading) cmd = (byte)Cmd.ATA_CMD_READ_PIO;
                else if (lba_mode == 2 && IsReading) cmd = (byte)Cmd.ATA_CMD_READ_PIO_EXT;
                else if (lba_mode == 0 && !IsReading) cmd = (byte)Cmd.ATA_CMD_WRITE_PIO;
                else if (lba_mode == 1 && !IsReading) cmd = (byte)Cmd.ATA_CMD_WRITE_PIO;
                else if (lba_mode == 2 && !IsReading) cmd = (byte)Cmd.ATA_CMD_WRITE_PIO_EXT;

                PortIO.Out8(CommandReg, cmd);

                if (IsReading)
                {
                    // PIO Read.
                    Poll(true);// Polling, set error and exit if there is.
                    PortIO.Read16(DataReg, xData);
                }
                else
                {
                    // PIO Write.
                    Poll(false);// Just Poll we don't want any error
                    PortIO.Write16(DataReg, xData);
                    switch (lba_mode)
                    {
                        case 0:
                        case 1:
                            PortIO.Out8(CommandReg, (byte)Cmd.ATA_CMD_CACHE_FLUSH);
                            break;
                        case 2:
                            PortIO.Out8(CommandReg, (byte)Cmd.ATA_CMD_CACHE_FLUSH_EXT);
                            break;
                    };
                    Poll(false);
                }

                // UnLock up device
                //Monitor.ReleaseLock(this);
                return true;
            }
            return false;
        }*/

        internal override bool Eject()
        {
            if (mDevice == Device.IDE_ATAPI)
            {
                // SCSI Packet Command
                mATAPI_Packet[0] = (byte)Cmd.ATAPI_CMD_EJECT;
                mATAPI_Packet[1] = 0x00;
                mATAPI_Packet[2] = 0x00;
                mATAPI_Packet[3] = 0x00;
                mATAPI_Packet[4] = 0x02;
                mATAPI_Packet[5] = 0x00;
                mATAPI_Packet[6] = 0x00;
                mATAPI_Packet[7] = 0x00;
                mATAPI_Packet[8] = 0x00;
                mATAPI_Packet[9] = 0x00;
                mATAPI_Packet[10] = 0x00;
                mATAPI_Packet[11] = 0x00;

                // Enable IRQ; Currently IRQ is not working...so we ignore it but very important
                IRQInvoked = false;
                PortIO.Out8(ControlReg, 0x0);

                SelectDrive();

                Send_SCSI_Package();
                return true;
            }

            return false;
        }

        private void Send_SCSI_Package()
        {
            // Tell Controller that we are sending package
            PortIO.Out8(CommandReg, (byte)Cmd.ATA_CMD_PACKET);

            // Wait till device get ready
            Poll(true);

            // Send SCSI-Packet command to controller
            PortIO.Write16(DataReg, mATAPI_Packet);

            // IRQ
            WaitIRQ();

            // Poll and check for error
            Poll(false);
        }

        private void Poll(bool AdvancedCheck)
        {
            // (I) Delay 400 nanosecond for BSY to be set:
            Wait();

            // (II) Wait for BSY to be cleared:
            // -------------------------------------------------
            while (((Status)PortIO.In8(StatusReg) & Status.ATA_SR_BSY) != 0)
                ; // Wait for BSY to be zero.

            if (AdvancedCheck)
            {
                var xState = (Status)PortIO.In8(StatusReg);

                // (III) Check For Errors:
                // -------------------------------------------------
                if ((xState & Status.ATA_SR_ERR) != 0)
                    throw new Exception("ATA Error");

                // (IV) Check If Device fault:
                // -------------------------------------------------
                if ((xState & Status.ATA_SR_DF) != 0)
                    throw new Exception("ATA Device Fault");

                // (V) Check DRQ:
                // -------------------------------------------------
                // BSY = 0; DF = 0; ERR = 0 so we should check for DRQ now.
                if ((xState & Status.ATA_SR_DRQ) == 0)
                    throw new Exception("ATA DRQ should be set");
            }
        }

        private void SelectDrive()
        {
            PortIO.Out8(DeviceSelect, (byte)((byte)mType << 4));
            Wait();
        }

        private void SelectDrive(byte head, bool lba)
        {
            PortIO.Out8(DeviceSelect, (byte)((lba ? 0xE0 : 0xA0) | ((byte)mType << 4) | head)); // Drive & LBA || Drive & CHS
            Wait();
        }

        private void Wait()
        {
            // reading status byte takes 100ns
            PortIO.In8(StatusReg);
            PortIO.In8(StatusReg);
            PortIO.In8(StatusReg);
            PortIO.In8(StatusReg);
        }

        private void WaitIRQ()
        {
            while (!IRQInvoked) ;
            IRQInvoked = false;
        }
    }
}

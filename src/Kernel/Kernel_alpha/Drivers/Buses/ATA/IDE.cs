using System;
using System.Collections.Generic;
using System.Text;
using Kernel_alpha.x86;
using Kernel_alpha.x86.Intrinsic;
using Kernel_alpha.Drivers;

namespace Kernel_alpha.Drivers.Buses.ATA
{
    public class IDE : BlockDevice
    {
        private IOPort DataReg;
        private IOPort FeatureReg;
        private IOPort CommandReg;
        private IOPort StatusReg;
        private IOPort AltStatusReg;
        private IOPort ControlReg;
        private IOPort SectorCountReg;

        private IOPort LBA0;
        private IOPort LBA1;
        private IOPort LBA2;

        private IOPort DeviceSelect;
        
        private UInt16 xBAR0;
        private UInt16 xBAR1;

        public DriveInfo DriveInfo;
        public bool IRQInvoked;

        public IDE(bool aSecondary, bool IsMaster = true)
        {
            xBAR0 = (ushort)(aSecondary ? 0x0170 : 0x01F0);
            xBAR1 = (ushort)(aSecondary ? 0x0376 : 0x03F6);

            DriveInfo.Channel = (aSecondary ? Channel.ATA_SECONDARY : Channel.ATA_PRIMARY);
            DriveInfo.Type = (aSecondary ? DeviceType.ATA_MASTER : DeviceType.ATA_SLAVE);

            Init();
        }

        private void Init()
        {
            DataReg = new IOPort((UInt16)(xBAR0 + (byte)Register.ATA_REG_DATA));
            FeatureReg = new IOPort((UInt16)(xBAR0 + (byte)Register.ATA_REG_FEATURES));
            SectorCountReg = new IOPort((UInt16)(xBAR0 + (byte)Register.ATA_REG_SECCOUNT0));
            CommandReg = new IOPort((UInt16)(xBAR0 + (byte)Register.ATA_REG_COMMAND));
            StatusReg = new IOPort((UInt16)(xBAR0 + (byte)Register.ATA_REG_STATUS));
            AltStatusReg = new IOPort((UInt16)(xBAR1 + (byte)Register.ATA_REG_ALTSTATUS));
            ControlReg = new IOPort((UInt16)(xBAR1 + (byte)Register.ATA_REG_CONTROL));

            LBA0 = new IOPort((UInt16)(xBAR0 + (byte)Register.ATA_REG_LBA0));
            LBA1 = new IOPort((UInt16)(xBAR0 + (byte)Register.ATA_REG_LBA1));
            LBA2 = new IOPort((UInt16)(xBAR0 + (byte)Register.ATA_REG_LBA2));

            DeviceSelect = new IOPort((UInt16)(xBAR0 + (byte)Register.ATA_REG_HDDEVSEL));
            DriveInfo = new ATA.DriveInfo();

            // 2- Disable IRQs:
            ControlReg.Byte = 0x2;

            //Discover what we have =P
            Discover();
        }

        /// <summary>
        /// This method discover the current ATA device and try to read all its configurations
        /// </summary>
        private void Discover()
        {
            DriveInfo.Device = Device.IDE_None;
            DriveInfo.BufferSize = 0;

            Status xStatus;
            bool Error = false;

            //Select Drive
            SelectDrive();

            //Send Identify command
            CommandReg.Byte = (byte)Cmd.ATA_CMD_IDENTIFY;
            Wait();

            if (StatusReg.Byte == 0)
                return; //No Device
            
            while (true)
            {
                xStatus = (Status)StatusReg.Byte;
                if ((xStatus & Status.ATA_SR_ERR) != 0)
                {
                    Error = true; // If Err, Device is not ATA.
                    break;
                }

                if (((xStatus & Status.ATA_SR_BSY) == 0) && ((xStatus & Status.ATA_SR_DRQ) != 0))
                    break; //Everything is fine
                Wait();
            }

            DriveInfo.Device = Device.IDE_ATA;
            DriveInfo.BufferSize = 512;

            // (IV) Probe for ATAPI Devices:
            if (Error)
            {
                ushort xTypeID = (ushort)(LBA2.Byte << 8 | LBA1.Byte);
                if (xTypeID == 0xEB14 || xTypeID == 0x9669)
                {
                    DriveInfo.Device = Device.IDE_ATAPI;
                    DriveInfo.BufferSize = 2048;
                }
                else
                {
                    DriveInfo.Device = Device.IDE_None;
                    DriveInfo.BufferSize = 0;
                    return;
                }

                //Send Identify packet command
                CommandReg.Byte = (byte)Cmd.ATA_CMD_IDENTIFY_PACKET;
                Wait();
            }
            
            var xBuff = new ushort[256];            
            DataReg.Read16(xBuff);
            
            //ATA/ATAPI COnfig
            DriveInfo.IsRemovable       = (xBuff[(int)Identify.ATA_IDENT_DEVICETYPE] & (1 << 7)) > 0;

            //CHS configurations
            DriveInfo.Cylinder          = xBuff.ToUInt32((int)Identify.ATA_IDENT_CYLINDERS);
            DriveInfo.Heads             = xBuff.ToUInt32((int)Identify.ATA_IDENT_HEADS);
            DriveInfo.SectorsPerTrack   = xBuff.ToUInt32((int)Identify.ATA_IDENT_SECTORS);
            DriveInfo.CommandSet        = xBuff.ToUInt32((int)Identify.ATA_IDENT_COMMANDSETS);

            ushort xFieldValid = xBuff[(int)Identify.ATA_IDENT_FIELDVALID];
            //1st bit determine weather it support LBA or not
            DriveInfo.LBASupport = (bool)((xFieldValid & 1) == 1);
            
            if ((DriveInfo.CommandSet & (1 << 26)) != 0)
                // Device uses 48-Bit Addressing:
                DriveInfo.Size = xBuff.ToUInt48((int)Identify.ATA_IDENT_MAX_LBA_EXT);
            else
                // Device uses CHS or 28-bit Addressing:
                DriveInfo.Size = xBuff.ToUInt32((int)Identify.ATA_IDENT_MAX_LBA);
            
            //Read Model, Firmware, SerialNo.
            DriveInfo.Model = xBuff.GetString((int)Identify.ATA_IDENT_MODEL, 40);
            DriveInfo.SerialNo = xBuff.GetString((int)Identify.ATA_IDENT_SERIAL, 20);

            /* Print Config...just for testing */
            /*Console.Write("Size:");
            Console.WriteLine(((UInt32)DriveInfo.Size).ToString());
            Console.Write("Model:");
            Console.WriteLine(DriveInfo.Model);
            Console.Write("Serial:");
            Console.WriteLine(DriveInfo.SerialNo);

            Console.Write("Type:");
            if (DriveInfo.Device == Device.IDE_ATA)
                Console.WriteLine("ATA");
            else if (DriveInfo.Device == Device.IDE_ATAPI)
                Console.WriteLine("ATAPI");*/
        }

        public override void Read(UInt32 SectorNo, uint SectorCount, byte[] xData)
        {
            Access_Disk(SectorNo, SectorCount, xData, true);
        }

        public override void Write(UInt32 SectorNo, uint SectorCount, byte[] xData)
        {
            Access_Disk(SectorNo, SectorCount, xData, false);
        }

        private byte[] xATAPI_Packet = new byte[12] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private void Access_Disk(UInt32 SectorNo, uint SectorCount, byte[] xData, bool IsReading)
        {
            if (DriveInfo.Device == Device.IDE_ATAPI)
            {
                /*
                 * Only 1 sector we can read at a time
                 */
                if (IsReading)
                {
                    //SCSI Packet Command
                    xATAPI_Packet[0] = (byte)Cmd.ATAPI_CMD_READ;
                    xATAPI_Packet[1] = 0x00;
                    xATAPI_Packet[2] = (byte)((SectorNo >> 24) & 0xFF);
                    xATAPI_Packet[3] = (byte)((SectorNo >> 16) & 0xFF);
                    xATAPI_Packet[4] = (byte)((SectorNo >> 8) & 0xFF);
                    xATAPI_Packet[5] = (byte)((SectorNo >> 0) & 0xFF);
                    xATAPI_Packet[6] = 0x00;
                    xATAPI_Packet[7] = 0x00;
                    xATAPI_Packet[8] = 0x00;
                    xATAPI_Packet[9] = (byte)(SectorCount & 0xFF);
                    xATAPI_Packet[10] = 0x00;
                    xATAPI_Packet[11] = 0x00;

                    //Enable IRQ
                    IRQInvoked = false;
                    ControlReg.Byte = 0x0;

                    SelectDrive();

                    FeatureReg.Byte = 0x0;//Tell controller that we are going to use PIO mode

                    //Tell constroller the size of each buffer
                    LBA1.Byte = (byte)((DriveInfo.BufferSize) & 0xFF);// Lower Byte of Sector Size. ATA_LBA_MID_PORT
                    LBA2.Byte = (byte)((DriveInfo.BufferSize >> 8) & 0xFF);// Upper Byte of Sector Size. ATA_LBA_HI_PORT          

                    //Send Packet command
                    Send_SCSI_Package();

                    //Actual size that is to transferred
                    UInt32 size = (UInt32)(LBA2.Byte << 8 | LBA1.Byte);

                    //Read the data
                    DataReg.Read16(xData);

                    WaitIRQ();
                    while (((Status)StatusReg.Byte & (Status.ATA_SR_BSY | Status.ATA_SR_DRQ)) != 0) ;
                }
            }
            else if (DriveInfo.Device == Device.IDE_ATA)
            {
                //Disable IRQ
                IRQInvoked = false;
                ControlReg.Byte = 0x2;

                ushort lba_mode, cyl;
                byte head, sect;
                byte[] lba_io = new byte[6];

                // (I) Select one from LBA28, LBA48 or CHS;
                if (SectorNo >= 0x10000000)
                {
                    // LBA48:
                    lba_mode = 2;
                    lba_io[0] = (byte)((SectorNo & 0x000000FF) >> 0);
                    lba_io[1] = (byte)((SectorNo & 0x0000FF00) >> 8);
                    lba_io[2] = (byte)((SectorNo & 0x00FF0000) >> 16);
                    lba_io[3] = (byte)((SectorNo & 0xFF000000) >> 24);
                    lba_io[4] = 0; // LBA28 is integer, so 32-bits are enough to access 2TB.
                    lba_io[5] = 0; // LBA28 is integer, so 32-bits are enough to access 2TB.
                    head = 0; // Lower 4-bits of HDDEVSEL are not used here.
                }
                else if ((DriveInfo.CommandSet & (1 << 26)) != 0)
                {
                    // LBA28:
                    lba_mode = 1;
                    lba_io[0] = (byte)((SectorNo & 0x00000FF) >> 0);
                    lba_io[1] = (byte)((SectorNo & 0x000FF00) >> 8);
                    lba_io[2] = (byte)((SectorNo & 0x0FF0000) >> 16);
                    lba_io[3] = 0; // These Registers are not used here.
                    lba_io[4] = 0; // These Registers are not used here.
                    lba_io[5] = 0; // These Registers are not used here.
                    head = (byte)((SectorNo & 0xF000000) >> 24);
                }
                else
                {
                    // CHS:
                    lba_mode = 0;
                    sect = (byte)((SectorNo % 63) + 1);
                    cyl = (ushort)((SectorNo + 1 - sect) / (16 * 63));
                    lba_io[0] = (byte)(sect & 0xFF);
                    lba_io[1] = (byte)((cyl >> 0) & 0xFF);
                    lba_io[2] = (byte)((cyl >> 8) & 0xFF);
                    lba_io[3] = 0;
                    lba_io[4] = 0;
                    lba_io[5] = 0;
                    head = (byte)((SectorNo + 1 - sect) % (16 * 63) / (63)); // Head number is written to HDDEVSEL lower 4-bits.
                }

                while (((Status)StatusReg.Byte & Status.ATA_SR_BSY) != 0) ;

                // (IV) Select Drive from the controller;
                if (lba_mode == 0)
                    SelectDrive(head, false);
                else
                    SelectDrive(head, true);

                // (V) Write Parameters;
                if (lba_mode == 2)
                {
                    throw new Exception("Yet to implement");
                }
                SectorCountReg.Byte = (byte)(SectorCount & 0xFF);
                LBA0.Byte = lba_io[0];
                LBA1.Byte = lba_io[1];
                LBA2.Byte = lba_io[2];

                //We are not using DMA so don't care about that
                byte cmd = 0;
                if (lba_mode == 0 && IsReading) cmd = (byte)Cmd.ATA_CMD_READ_PIO;
                else if (lba_mode == 1 && IsReading) cmd = (byte)Cmd.ATA_CMD_READ_PIO;
                else if (lba_mode == 2 && IsReading) cmd = (byte)Cmd.ATA_CMD_READ_PIO_EXT;
                else if (lba_mode == 0 && !IsReading) cmd = (byte)Cmd.ATA_CMD_WRITE_PIO;
                else if (lba_mode == 1 && !IsReading) cmd = (byte)Cmd.ATA_CMD_WRITE_PIO;
                else if (lba_mode == 2 && !IsReading) cmd = (byte)Cmd.ATA_CMD_WRITE_PIO_EXT;

                CommandReg.Byte = cmd;

                if (IsReading)
                {
                    // PIO Read.
                    Poll(true);// Polling, set error and exit if there is.
                    DataReg.Read16(xData);
                }
                else
                {
                    // PIO Write.
                    Poll(false);//Just Poll we don't want any error
                    DataReg.Write16(xData);
                    switch (lba_mode)
                    {
                        case 0:
                        case 1:
                            CommandReg.Byte = (byte)Cmd.ATA_CMD_CACHE_FLUSH;
                            break;
                        case 2:
                            CommandReg.Byte = (byte)Cmd.ATA_CMD_CACHE_FLUSH_EXT;
                            break;
                    };
                    Poll(false);
                }
            }
        }
        
        public void Eject()
        {
            if (DriveInfo.Device == Device.IDE_ATAPI)
            {
                //SCSI Packet Command
                xATAPI_Packet[0] = (byte)Cmd.ATAPI_CMD_EJECT;
                xATAPI_Packet[1] = 0x00;
                xATAPI_Packet[2] = 0x00;
                xATAPI_Packet[3] = 0x00;
                xATAPI_Packet[4] = 0x02;
                xATAPI_Packet[5] = 0x00;
                xATAPI_Packet[6] = 0x00;
                xATAPI_Packet[7] = 0x00;
                xATAPI_Packet[8] = 0x00;
                xATAPI_Packet[9] = 0x00;
                xATAPI_Packet[10] = 0x00;
                xATAPI_Packet[11] = 0x00;
                
                //Enable IRQ; Currently IRQ is not working...so we ignore it but very important                
                IRQInvoked = false;
                ControlReg.Byte = 0x0;

                SelectDrive();

                Send_SCSI_Package();
            }
        }

        private void Send_SCSI_Package()
        {
            //Tell Controller that we are sending package
            CommandReg.Byte = (byte)Cmd.ATA_CMD_PACKET;

            //Wait till device get ready
            Poll(true);

            //Send SCSI-Packet command to controller
            DataReg.Write16(xATAPI_Packet);

            //IRQ
            WaitIRQ();

            //Poll and check for error
            Poll(false);
        }

        private void Poll(bool AdvancedCheck)
        {
            // (I) Delay 400 nanosecond for BSY to be set:
            Wait();

            // (II) Wait for BSY to be cleared:
            // -------------------------------------------------
            while (((Status)StatusReg.Byte & Status.ATA_SR_BSY) != 0)
                ; // Wait for BSY to be zero.

            if (AdvancedCheck)
            {
                var xState = (Status)StatusReg.Byte;

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
        /// <summary>
        /// 0 - Master
        /// 1 - Slave
        /// </summary>
        /// <param name="disk"></param>
        private void SelectDrive()
        {
            DeviceSelect.Byte = (byte)((byte)DriveInfo.Type << 4);
            Wait();
        }

        private void SelectDrive(byte head, bool lba)
        {
            if (!lba)
                DeviceSelect.Byte = (byte)(0xA0 | ((byte)DriveInfo.Type << 4) | head); // Drive & CHS.
            else
                DeviceSelect.Byte = (byte)(0xE0 | ((byte)DriveInfo.Type << 4) | head); // Drive & LBA
            Wait();
        }

        private void Wait()
        {
            //reading status byte takes 100ns
            byte n;
            n = StatusReg.Byte;
            n = StatusReg.Byte;
            n = StatusReg.Byte;
            n = StatusReg.Byte;
        }

        private void WaitIRQ()
        {
            while (!IRQInvoked) ;
            IRQInvoked = false;
        }
    }    
}

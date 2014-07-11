using System;
using System.Collections.Generic;
using System.Text;
using Kernel_alpha.x86;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha.Drivers.Buses.ATA
{
    public class IDE
    {
        private IOPort DataReg;
        private IOPort ErrorReg;
        private IOPort CommandReg;
        private IOPort StatusReg;
        private IOPort AltStatusReg;
        private IOPort ControlReg;
        private IOPort SectorCountReg;

        private IOPort LBA0;
        private IOPort LBA1;
        private IOPort LBA2;

        private IOPort DeviceSelect;

        public readonly Channel Channel;

        private UInt16 xBAR0;
        private UInt16 xBAR1;

        public DriveInfo DriveInfo;
        
        public IDE(bool aSecondary)
        {
            xBAR0 = (ushort)(aSecondary ? 0x0170 : 0x01F0);
            xBAR1 = (ushort)(aSecondary ? 0x0376 : 0x03F6);
            
            Channel = (aSecondary ? Channel.ATA_SECONDARY : Channel.ATA_PRIMARY);
            
            Init();
        }

        private void Init()
        {
            DataReg = new IOPort((UInt16)(xBAR0 + (byte)Register.ATA_REG_DATA));
            ErrorReg = new IOPort((UInt16)(xBAR0 + (byte)Register.ATA_REG_ERROR));
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
            DriveInfo.Device = Device.IDE_ATA;

            Status xStatus;
            bool Error = false;

            #warning READ COMMENT BELOW
            //Select Drive
            SelectDrive(0);//FIXME: 0--> Master | 1--> Slave

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
            
            // (IV) Probe for ATAPI Devices:
            if (Error)
            {
                ushort xTypeID = (ushort)(LBA2.Byte << 8 | LBA1.Byte);
                if (xTypeID == 0xEB14 || xTypeID == 0x9669)
                    DriveInfo.Device = Device.IDE_ATAPI;
                else
                    return;

                //Send Identify packet command
                CommandReg.Byte = (byte)Cmd.ATA_CMD_IDENTIFY_PACKET;
                Wait();
            }
            
            var xBuff = new ushort[256];            
            DataReg.Read16(xBuff);
            
            //CHS configurations
            DriveInfo.Cylinder      = xBuff.ToUInt32((int)Identify.ATA_IDENT_CYLINDERS);
            DriveInfo.Heads         = xBuff.ToUInt32((int)Identify.ATA_IDENT_HEADS);
            DriveInfo.Sectors       = xBuff.ToUInt32((int)Identify.ATA_IDENT_SECTORS);
            DriveInfo.CommandSet    = xBuff.ToUInt32((int)Identify.ATA_IDENT_COMMANDSETS);

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
            Console.Write("Size:");
            Console.WriteLine((DriveInfo.Size).ToString());
            Console.Write("Model:");
            Console.WriteLine(DriveInfo.Model);
            Console.Write("Serial:");
            Console.WriteLine(DriveInfo.SerialNo);

            Console.Write("Type:");
            if (DriveInfo.Device == Device.IDE_ATA)
                Console.WriteLine("ATA");
            else if (DriveInfo.Device == Device.IDE_ATAPI)
                Console.WriteLine("ATAPI");
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
                if ((xState & Status.ATA_SR_DRQ) != 0)
                    throw new Exception("ATA DRQ should be set");
            }
        }
        /// <summary>
        /// 0 - Master
        /// 1 - Slave
        /// </summary>
        /// <param name="disk"></param>
        private void SelectDrive(byte disk)
        {
            DeviceSelect.Byte = (byte)(0xA0 | disk << 4);
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
    }    
}

/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          IDE Driver Helper functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomix.Kernel_H.Drivers.buses.ATA
{
    public enum Channel : byte
    {
        PRIMARY = 0x0,
        SECONDARY = 0x1
    }

    public enum Type : byte
    {
        MASTER = 0x0,
        SLAVE = 0x1
    }

    public enum Status : byte
    {
        ATA_SR_BSY = 0x80,
        ATA_SR_DRDY = 0x40,
        ATA_SR_DF = 0x20,
        ATA_SR_DSC = 0x10,
        ATA_SR_DRQ = 0x08,
        ATA_SR_CORR = 0x04,
        ATA_SR_IDX = 0x02,
        ATA_SR_ERR = 0x01
    };

    public enum Device : byte
    {
        IDE_ATA = 0x1,
        IDE_ATAPI = 0x2,
        IDE_None = 0x0
    };

    public enum Register : byte
    {
        ATA_REG_DATA = 0x00,
        ATA_REG_ERROR = 0x01,
        ATA_REG_FEATURES = 0x01,
        ATA_REG_SECCOUNT0 = 0x02,
        ATA_REG_LBA0 = 0x03,
        ATA_REG_LBA1 = 0x04,
        ATA_REG_LBA2 = 0x05,
        ATA_REG_HDDEVSEL = 0x06,
        ATA_REG_COMMAND = 0x07,
        ATA_REG_STATUS = 0x07,
        ATA_REG_SECCOUNT1 = 0x08,
        ATA_REG_LBA3 = 0x09,
        ATA_REG_LBA4 = 0x0A,
        ATA_REG_LBA5 = 0x0B,
        ATA_REG_CONTROL = 0x0C,
        ATA_REG_ALTSTATUS = 0x0C,
        ATA_REG_DEVADDRESS = 0x0D
    };

    public enum Cmd : byte
    {
        ATA_CMD_READ_PIO = 0x20,
        ATA_CMD_READ_PIO_EXT = 0x24,
        ATA_CMD_READ_DMA = 0xC8,
        ATA_CMD_READ_DMA_EXT = 0x25,
        ATA_CMD_WRITE_PIO = 0x30,
        ATA_CMD_WRITE_PIO_EXT = 0x34,
        ATA_CMD_WRITE_DMA = 0xCA,
        ATA_CMD_WRITE_DMA_EXT = 0x35,
        ATA_CMD_CACHE_FLUSH = 0xE7,
        ATA_CMD_CACHE_FLUSH_EXT = 0xEA,
        ATA_CMD_PACKET = 0xA0,
        ATA_CMD_IDENTIFY_PACKET = 0xA1,
        ATA_CMD_IDENTIFY = 0xEC,
        ATAPI_CMD_READ = 0xA8,
        ATAPI_CMD_EJECT = 0x1B
    };

    public enum Identify : byte
    {
        ATA_IDENT_DEVICETYPE = 0,
        ATA_IDENT_CYLINDERS = 1,
        ATA_IDENT_HEADS = 3,
        ATA_IDENT_SECTORS = 6,
        ATA_IDENT_SERIAL = 10,
        ATA_IDENT_MODEL = 27,
        ATA_IDENT_CAPABILITIES = 49,
        ATA_IDENT_FIELDVALID = 53,
        ATA_IDENT_MAX_LBA = 60,
        ATA_IDENT_COMMANDSETS = 82,
        ATA_IDENT_MAX_LBA_EXT = 100
    };

    internal static class misc
    {
        internal static UInt32 ToUInt32(this ushort[] xBuff, int loc)
        {
            return (UInt32)(xBuff[loc + 1] << 16 | xBuff[loc]);
        }

        internal static UInt64 ToUInt64(this ushort[] xBuff, int loc)
        {
            return (UInt64)(xBuff[loc + 3] << 48 | xBuff[loc + 2] << 32 | xBuff[loc + 1] << 16 | xBuff[loc]);
        }

        internal static UInt64 ToUInt48(this ushort[] xBuff, int loc)
        {
            return (UInt64)(xBuff[loc + 2] << 32 | xBuff[loc + 1] << 16 | xBuff[loc]);
        }

        internal static string GetString(this ushort[] xBuff, int loc, int length)
        {
            char[] xResult = new char[length];
            for (int k = 0; k < (length / 2); k++)
            {
                xResult[k * 2] = (char)((xBuff[loc + k] >> 8) & 0xFF);
                xResult[k * 2 + 1] = (char)(xBuff[loc + k] & 0xFF);
            }
            return new String(xResult);
        }
    }
}

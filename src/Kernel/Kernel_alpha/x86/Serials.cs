using System;
using System.Collections.Generic;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha.x86
{
    public static class Serials
    {
        public enum Port : uint
        {
            Com1 = 0x3F8,
            Com2 = 0x2F8,
            Com3 = 0x3E8,
            Com4 = 0x2F8
        };

        public enum Cmd : uint
        {
            COM_Data = 0x00,
            COM_Interrupt = 0x01,
            COM_LineControl = 0x02,
            COM_ModemControl = 0x03,
            COM_LineStatus = 0x04,
            COM_ModemStatus = 0x05,
            COM_Scratch = 0x06
        };

        public static void SetupPort(Port PORT = Port.Com1)
        {
            IOPort.Outb((ushort)(PORT + (ushort)Cmd.COM_Interrupt), 0x00);        // Disable all interrupts
            IOPort.Outb((ushort)(PORT + (ushort)Cmd.COM_ModemControl), 0x80);     // Enable DLAB (set baud rate divisor)
            IOPort.Outb((ushort)(PORT + (ushort)Cmd.COM_Data), 0x03);             // Set divisor to 3 (lo byte) 38400 baud
            IOPort.Outb((ushort)(PORT + (ushort)Cmd.COM_Interrupt), 0x00);        //                  (hi byte)
            IOPort.Outb((ushort)(PORT + (ushort)Cmd.COM_ModemControl), 0x03);     // 8 bits, no parity, one stop bit
            IOPort.Outb((ushort)(PORT + (ushort)Cmd.COM_LineControl), 0xC7);      // Enable FIFO, clear them, with 14-byte threshold
            IOPort.Outb((ushort)(PORT + (ushort)Cmd.COM_LineStatus), 0x0B);       // IRQs enabled, RTS/DSR set
            IOPort.Outb((ushort)(PORT + (ushort)Cmd.COM_Interrupt), 0x0F);
        }

        private static void WaitForWriteReady(Port PORT)
        {
            while ((IOPort.Inb((ushort)(PORT + (ushort)Cmd.COM_ModemStatus)) & 0x20) == 0x0)
            {
                Thread.Sleep(15);
            }
        }

        public static void Write(byte a, Port PORT = Port.Com1)
        {
            WaitForWriteReady(PORT);
            IOPort.Outb((ushort)PORT, a);
        }

        public static void Write(byte[] xData, Port PORT = Port.Com1)
        {
            for (int i = 0; i < xData.Length; i++)
                Write(xData[i], PORT);
        }
    }
}

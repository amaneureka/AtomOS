/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          It will flush debug text to Com serial port
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.Arch.x86;

namespace Atomix.Kernel_H.Core
{
    internal static class Debug
    {
        internal enum Port : uint
        {
            Com1 = 0x3F8,
            Com2 = 0x2F8,
            Com3 = 0x3E8,
            Com4 = 0x2F8
        };

        internal enum Cmd : uint
        {
            COM_Data = 0x00,
            COM_Interrupt = 0x01,
            COM_LineControl = 0x02,
            COM_ModemControl = 0x03,
            COM_LineStatus = 0x04,
            COM_ModemStatus = 0x05,
            COM_Scratch = 0x06
        };

        static uint Lock;

        internal static void Init()
        {
            var PORT = Port.Com1;
            PortIO.Out8((ushort)(PORT + (ushort)Cmd.COM_Interrupt), 0x00);        // Disable all interrupts
            PortIO.Out8((ushort)(PORT + (ushort)Cmd.COM_ModemControl), 0x80);     // Enable DLAB (set baud rate divisor)
            PortIO.Out8((ushort)(PORT + (ushort)Cmd.COM_Data), 0x03);             // Set divisor to 3 (lo byte) 38400 baud
            PortIO.Out8((ushort)(PORT + (ushort)Cmd.COM_Interrupt), 0x00);        //                  (hi byte)
            PortIO.Out8((ushort)(PORT + (ushort)Cmd.COM_ModemControl), 0x03);     // 8 bits, no parity, one stop bit
            PortIO.Out8((ushort)(PORT + (ushort)Cmd.COM_LineControl), 0xC7);      // Enable FIFO, clear them, with 14-byte threshold
            PortIO.Out8((ushort)(PORT + (ushort)Cmd.COM_LineStatus), 0x0B);       // IRQs enabled, RTS/DSR set
            PortIO.Out8((ushort)(PORT + (ushort)Cmd.COM_Interrupt), 0x0F);

            Write("Debugger Initalized\n");
        }

        private static void WaitForWriteReady()
        {
            while ((PortIO.In8((ushort)(Port.Com1 + (ushort)Cmd.COM_ModemStatus)) & 0x20) == 0x0) ;
        }

        internal static void Write(string str, int nums)
        {
            Write(str, (uint)nums);
        }

        internal static void Write(string str, uint nums)
        {
            Monitor.AcquireLock(ref Lock);

            char a;
            for (int i = 0; i < str.Length; i++)
            {
                a = str[i];
                if (a == '%' && str[i + 1] == 'd')
                {
                    Write(nums);
                    i++;
                }
                else
                {
                    Write(a);
                }
            }

            Monitor.ReleaseLock(ref Lock);
        }

        internal static void Write(string str, string arg0)
        {
            Monitor.AcquireLock(ref Lock);

            char a;
            for (int i = 0; i < str.Length; i++)
            {
                a = str[i];
                if (a == '%' && str[i + 1] == 's')
                {
                    WriteAsync(arg0);
                    i++;
                }
                else
                {
                    Write(a);
                }
            }
            Monitor.ReleaseLock(ref Lock);
        }

        internal static void Write(string str)
        {
            Monitor.AcquireLock(ref Lock);
            WriteAsync(str);
            Monitor.ReleaseLock(ref Lock);
        }

        private static void WriteAsync(string str)
        {
            for (int i = 0; i < str.Length; i++)
                Write(str[i]);
        }

        private static void Write(char a)
        {
            Write((byte)a);
        }

        public static void Write(byte a)
        {
            WaitForWriteReady();
            PortIO.Out8((ushort)Port.Com1, a);
        }

        private static void Write(uint a)
        {
            uint tmp = a, c = 1;

            while (tmp > 9)
            {
                c *= 10;
                tmp /= 10;
            }

            tmp = a;
            while (c > 0)
            {
                Write((byte)('0' + (char)(tmp / c)));
                tmp %= c;
                c /= 10;
            }
        }
    }
}

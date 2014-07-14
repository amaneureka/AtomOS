using System;

using libAtomixH.IO.Ports;

namespace libAtomixH.Drivers.Input.PS2
{
    public static class PS2Ports
    {
        public static IOPort ps2;

        static PS2Ports ()
        {
            ps2 = new IOPort ((ushort)ps2Port.PS2_Cmd);
        }

        public enum ps2Port : ushort
        {
            PS2_Cmd = 0x60,
            PS2_Data = 0x64
        };

        public enum ps2Cmd : byte
        {
            Key_LEDs = 0xED,
        };

        public enum ps2Res : byte
        {
            Acknowledged = 0xFA,
        };

        public static void SendCommand (ps2Cmd cmd, byte data)
        {
            ps2.Byte = (byte)cmd;
            WaitForResponse ();
            ps2.Byte = data;
            WaitForResponse ();
        }

        private static void WaitForResponse ()
        {
            while (ps2.Byte != (byte)ps2Res.Acknowledged) ;
        }
    }
}

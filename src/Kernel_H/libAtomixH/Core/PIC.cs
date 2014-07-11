using System;

using libAtomixH.IO.Ports;

namespace libAtomixH.Core
{
    public static class PIC
    {
        private const byte PIC1_Command = 0x20;
        private const byte PIC2_Command = 0xA0;

        private const byte PIC1_Data = 0x21;
        private const byte PIC2_Data = 0xA1;

        private const byte ICW1_ICW4 = 0x01;
        private const byte ICW1_SingleCascadeMode = 0x02;
        private const byte ICW1_Interval4 = 0x04;
        private const byte ICW1_LevelTriggeredEdgeMode = 0x08;
        private const byte ICW1_Initialization = 0x10;
        private const byte ICW2_MasterOffset = 0x20;
        private const byte ICW2_SlaveOffset = 0x28;
        private const byte ICW4_8086 = 0x01;
        private const byte ICW4_AutoEndOfInterrupt = 0x02;
        private const byte ICW4_BufferedSlaveMode = 0x08;
        private const byte ICW4_BufferedMasterMode = 0x0C;
        private const byte ICW4_SpecialFullyNested = 0x10;

        private const byte EOI = 0x20;

        public static void Setup ()
        {
            Remap (0x20, 0xF9 | 0x08, 0x28, 0xEF);
        }

        private static void Remap (byte masterStart, byte masterMask, byte slaveStart, byte slaveMask)
        {
            Native.Out8 (PIC1_Command, ICW1_Initialization + ICW1_ICW4);
            Native.Wait ();
            Native.Out8 (PIC2_Command, ICW1_Initialization + ICW1_ICW4);
            Native.Wait ();
            Native.Out8 (PIC1_Data, masterStart);
            Native.Wait ();
            Native.Out8 (PIC2_Data, slaveStart);
            Native.Wait ();

            Native.Out8 (PIC1_Data, 4);
            Native.Wait ();
            Native.Out8 (PIC2_Data, 2);
            Native.Wait ();

            // set modes:
            Native.Out8 (PIC1_Data, ICW4_8086);
            Native.Wait ();
            Native.Out8 (PIC2_Data, ICW4_8086);
            Native.Wait ();

            // set masks:
            Native.Out8 (PIC1_Data, masterMask);
            Native.Wait ();
            //Native.Out8(PIC2_Data, slaveMask);
            ///Native.Wait();
        }

        public static void SendEndOfInterrupt (byte irq)
        {
            if (irq >= 40) // or untranslated IRQ >= 8
                Native.Out8 (PIC2_Command, EOI);

            Native.Out8 (PIC1_Command, EOI);
        }

        public static void ClearMask (ushort IRQline)
        {
            ushort port;
            byte value;

            if (IRQline < 8)
            {
                port = 0x20 + 1;
            }
            else
            {
                port = 0xA0 + 1;
                IRQline -= 8;
            }
            value = (byte)(Native.In8 (port) & ~(1 << IRQline));
            Native.Out8 (port, value);
        }
    }
}

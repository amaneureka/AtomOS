/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Programmable Interrupt Controller 8086 Chip Handler, basically remapping IRQs
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.arch.x86
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

        public static void Setup()
        {
            Remap(0x20, 0xF9, 0x28, 0xFF);
        }

        private static void Remap(byte masterStart, byte masterMask, byte slaveStart, byte slaveMask)
        {
            PortIO.Out8(PIC1_Command, ICW1_Initialization + ICW1_ICW4);
            PortIO.Wait();
            PortIO.Out8(PIC2_Command, ICW1_Initialization + ICW1_ICW4);
            PortIO.Wait();
            PortIO.Out8(PIC1_Data, masterStart);
            PortIO.Wait();
            PortIO.Out8(PIC2_Data, slaveStart);
            PortIO.Wait();

            PortIO.Out8(PIC1_Data, 4);
            PortIO.Wait();
            PortIO.Out8(PIC2_Data, 2);
            PortIO.Wait();

            // set modes:
            PortIO.Out8(PIC1_Data, ICW4_8086);
            PortIO.Wait();
            PortIO.Out8(PIC2_Data, ICW4_8086);
            PortIO.Wait();

            // set masks:
            PortIO.Out8(PIC1_Data, masterMask);
            PortIO.Wait();
            //PortIO.Out8(PIC2_Data, slaveMask);
            //PortIO.Wait();
        }

        public static void EndOfInterrupt(uint irq)
        {
            if (irq >= 40) // or untranslated IRQ >= 8
                PortIO.Out8(PIC2_Command, EOI);

            PortIO.Out8(PIC1_Command, EOI);
        }
    }
}

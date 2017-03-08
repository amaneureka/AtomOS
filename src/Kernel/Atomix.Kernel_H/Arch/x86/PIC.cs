/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Programmable Interrupt Controller 8086 Chip Handler, basically remapping IRQs
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.Arch.x86
{
    internal static class PIC
    {
        const byte PIC1_Command = 0x20;
        const byte PIC2_Command = 0xA0;

        const byte PIC1_Data = 0x21;
        const byte PIC2_Data = 0xA1;

        const byte ICW1_ICW4 = 0x01;
        const byte ICW1_SingleCascadeMode = 0x02;
        const byte ICW1_Interval4 = 0x04;
        const byte ICW1_LevelTriggeredEdgeMode = 0x08;
        const byte ICW1_Initialization = 0x10;
        const byte ICW2_MasterOffset = 0x20;
        const byte ICW2_SlaveOffset = 0x28;
        const byte ICW4_8086 = 0x01;
        const byte ICW4_AutoEndOfInterrupt = 0x02;
        const byte ICW4_BufferedSlaveMode = 0x08;
        const byte ICW4_BufferedMasterMode = 0x0C;
        const byte ICW4_SpecialFullyNested = 0x10;

        const byte EOI = 0x20;

        internal static void Setup()
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

        internal static void EndOfInterrupt(int irq)
        {
            if (irq >= 40) // or untranslated IRQ >= 8
                PortIO.Out8(PIC2_Command, EOI);

            PortIO.Out8(PIC1_Command, EOI);
        }
    }
}

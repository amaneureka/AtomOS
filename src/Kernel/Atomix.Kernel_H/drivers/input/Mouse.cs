﻿/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          PS2 mouse driver
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.io;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.drivers.input
{
    internal static class Mouse
    {
        const byte MOUSE_PORT = 0x60;
        const byte MOUSE_STATUS = 0x64;
        const byte MOUSE_ABIT = 0x02;
        const byte MOUSE_BBIT = 0x01;
        const byte MOUSE_WRITE = 0xD4;
        const byte MOUSE_F_BIT = 0x20;
        const byte MOUSE_V_BIT = 0x08;

        const byte LEFT_CLICK = 0x1;
        const byte RIGHT_CLICK = 0x2;
        const byte MIDDLE_CLICK = 0x4;

        public const byte MOUSE_MAGIC = 0xAC;

        internal static Pipe MousePipe;

        internal static void Setup()
        {
            Debug.Write("PS/2 Mouse Controller Setup\n");
            MouseCycle = 0;
            MouseData = new byte[4];
            MouseData[0] = MOUSE_MAGIC;
            MousePipe = new Pipe(4, 1024);
            IDT.RegisterInterrupt(HandleIRQ, 0x2C);
            
            Wait(true);
            PortIO.Out8(MOUSE_STATUS, 0xA8);
            Wait(true);
            PortIO.Out8(MOUSE_STATUS, 0x20);
            Wait(false);

            byte status = (byte)(PortIO.In8(MOUSE_PORT) | 2);
            Wait(true);
            PortIO.Out8(MOUSE_STATUS, 0x60);
            Wait(true);
            PortIO.Out8(MOUSE_PORT, status);
            Write(0xF6);
            Read();
            Write(0xF4);
            Read();

            Debug.Write("Mouse Done\n");
        }

        static uint MouseCycle;
        static byte[] MouseData;

        private static void HandleIRQ(ref IRQContext context)
        {
            byte input = Read();
            switch (MouseCycle)
            {
                case 0:
                    {
                        MouseData[1] = input;
                        if ((input & MOUSE_V_BIT) != 0)
                            MouseCycle = 1;
                    }
                    break;
                case 1:
                    {
                        MouseData[2] = input;
                        MouseCycle = 2;
                    }
                    break;
                case 2:
                    {
                        MouseData[3] = input;
                        MouseCycle = 0;
                        /*
                         * http://wiki.osdev.org/Mouse_Input
                         * The top two bits of the first byte (values 0x80 and 0x40) supposedly show Y and X overflows, 
                         * respectively. They are not useful. If they are set, you should probably just discard the entire packet.
                         */
                        //if ((MouseData[1] & 0x40) != 0)//X Overflow
                        //    break;
                        //if ((MouseData[1] & 0x80) != 0)//Y Overflow
                        //    break;
                        //Send packet to kernel:= { MAGIC, btn, X-Pos, Y-Pos }
                        MousePipe.Write(MouseData, false);//Send package and seek the read pointer if necessary
                    }
                    break;
            }
        }

        private static void Write(byte data)
        {
            Wait(true);
            PortIO.Out8(MOUSE_STATUS, MOUSE_WRITE);
            Wait(true);
            PortIO.Out8(MOUSE_PORT, data);
        }

        private static byte Read()
        {
            Wait(false);
            return PortIO.In8(MOUSE_PORT);
        }

        private static void Wait(bool type)
        {
            UInt32 timeout = 100000;
            if (!type)
            {
                while (--timeout > 0)
                {
                    if ((PortIO.In8(MOUSE_STATUS) & MOUSE_BBIT) != 0)
                        return;
                }
                Debug.Write("[mouse]: TIMEOUT\n");
                return;
            }
            else
            {
                while (--timeout > 0)
                {
                    if ((PortIO.In8(MOUSE_STATUS) & MOUSE_ABIT) == 0)
                        return;
                }
                Debug.Write("[mouse]: TIMEOUT\n");
                return;
            }
        }
    }
}

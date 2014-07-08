using System;
using Kernel_alpha.x86;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha.Drivers.Input.Mouse
{
    // PS/2 Mouse Interface stuff

    public class PS2
    {
        /// <summary>
        /// The X coordinate
        /// </summary>
        public int X = 0;

        /// <summary>
        /// The Y coordinate
        /// </summary>
        public int Y = 0;

        /// <summary>
        /// The mouse buttons
        /// </summary>
        MouseButtons Buttons = MouseButtons.None;

        // We'll be using 40 as sample rate
        // That's not too slow and not too fast
        int SampleRate = 40;

        // We'll be receiving packets from here
        IOPort Data = null;

        // We'll be using this port to check if
        // packets are available
        IOPort Poll = null;

        byte cycle = 0;
        int[] packet = new int[4];

        public PS2 ()
        {
            // I guess we'll be receiving data from Port 0x60
            Data = new IOPort (0x60);

            // And polling port 0x64
            Poll = new IOPort (0x64);
        }

        public void Initialize ()
        {
            // Enable the Aux Input
            WaitSignal ();
            Poll.Byte = (byte)MouseCommandSet.Enable;

            // Pretty self-explanatory
            EnableInterrupt ();

            // Set defaults
            SendCommand (MouseCommandSet.SetDefaults);

            // Enable the mouse
            SendCommand (MouseCommandSet.EnablePacketStreaming);           
        }

        public void EnableInterrupt ()
        {
            WaitSignal ();
            Poll.Byte = 0x20;
            WaitData ();
            byte status = (byte)(Data.Byte | 2);
            WaitSignal ();
            Data.Byte = status;
        }

        public void HandleIRQ ()
        {
            switch (cycle)
            {
                case 0:
                    packet[0] = Data.Byte;

                    if ((packet[0] & 0x8) == 0x8)
                        cycle++;

                    break;

                case 1:
                    packet[1] = Data.Byte;
                    cycle++;

                    break;

                case 2:
                    packet[2] = Data.Byte;
                    cycle = 0;

                    if ((packet[0] & 0x10) == 0x10)
                        X -= packet[1] ^ 0xFF;
                    else
                        X += packet[1];

                    if ((packet[0] & 0x20) == 0x20)
                        Y += packet[2] ^ 0xFF;
                    else
                        Y -= packet[2];

                    if (X < 0)
                        X = 0;
                    else if (X > 319)
                        X = 319;

                    if (Y < 0)
                        Y = 0;
                    else if (Y > 199)
                        Y = 199;

                    Buttons = (MouseButtons)(packet[0] & 0x7);

                    break;
            }
        }

        public void SendCommand (MouseCommandSet cmd)
        {
            // Wait till we can send a command
            WaitSignal ();

            // Tell the poll port that we
            // want to send a command to the mouse
            Poll.Byte = (byte)MouseCommandSet.Send;

            // Wait till we can send a command
            WaitSignal ();

            // Send the command to the data port
            Data.Byte = (byte)cmd;

            //Wait till it respond, Have to check if ACK
            Read();
        }

        public byte Read ()
        {
            // Wait for data to arrive
            WaitData ();

            // Return the data
            return Data.Byte;
        }

        public void WaitData ()
        {
            for (int i = 0; i < 1000 & ((Poll.Byte & 1) == 1); i++)
            {
                // Do nothing
            }
        }

        public void WaitSignal ()
        {
            for (int i = 0; i < 1000 & ((Poll.Byte & 2) != 0); i++)
            {
                // Do nothing
            }
        }

        public enum MouseCommandSet : byte
        {
            // Acknowledged
            Ack = 0xFA,

            // Send data
            Send = 0xD4,

            // Enable PS/2
            Enable = 0xA8,

            // Basic instruction sets
            Reset = 0xFF,
            Resend = 0xFE,
            SetDefaults = 0xF6,
            DisablePacketStreaming = 0xF5,
            EnablePacketStreaming = 0xF4,
            SetSampleRate = 0xF3,
            GetMouseID = 0xF2,
            RequestSinglePacket = 0xEB,
            StatusRequest = 0xE9,
            SetResolution = 0xE8,

            // Not really useful
            // but for the sake of completeness
            SetRemoteMode = 0xF0,
            SetWrapMode = 0xEE,
            ResetWrapMode = 0xEC,
            SetSteamMode = 0xEA,
            SetScaling21 = 0xE7,
            SetScaling11 = 0xE6
        }

        public enum MouseButtons : byte
        {
            None = 0,
            Left = 1,
            Right = 2,
            Middle = 4
        }
    }
}

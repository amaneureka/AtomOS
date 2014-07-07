using System;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha.Drivers.Input.Mouse
{
    // PS/2 Mouse Interface stuff

    public class PS2
    {
        // We'll be using 40 as sample rate
        // That's not too slow and not too fast
        int SampleRate = 40;

        // We'll be receiving packets from here
        IOPort Data = null;

        // We'll be using this port to check if
        // packets are available
        IOPort Poll = null;

        public void Initialize ()
        {
            // I guess we'll be receiving data from Port 0x60
            Data = new IOPort (0x60);

            // And polling port 0x64
            Poll = new IOPort (0x64);

            // Now to the configuration part

            // Enable the Aux Input
            WaitSignal ();
            Poll.Byte = (byte)MouseCommandSet.Enable;

            // We could map the mouse to IRQ 12 here
            // but I won't do it currently cause I don't
            // really know how the AtomixOS handles interrupts

            // Set defaults
            SendCommand (MouseCommandSet.SetDefaults);

            // Enable the mouse
            SendCommand (MouseCommandSet.EnablePacketStreaming);
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

        enum MouseCommandSet : byte
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
    }
}

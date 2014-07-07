using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.x86.Intrinsic
{
    // Awesome IOPorts :)
    public class IOPort
    {
        public ushort Address;

        public IOPort (ushort addr)
        {
            Address = addr;
        }

        public byte Inb ()
        {
            return Byte;
        }

        public static byte Inb (ushort addr)
        {
            return Native.In8 (addr);
        }

        public ushort Inw ()
        {
            return Word;
        }

        public static ushort Inw (ushort addr)
        {
            return Native.In16 (addr);
        }

        public uint Inl ()
        {
            return DWord;
        }

        public static uint Inl (ushort addr)
        {
            return Native.In32 (addr);
        }

        public void Outb (byte val)
        {
            Byte = val;
        }

        public static void Outb (ushort addr, byte val)
        {
            Native.Out8 (addr, val);
        }

        public void Outw (ushort val)
        {
            Word = val;
        }

        public static void Outw (ushort addr, ushort val)
        {
            Native.Out16 (addr, val);
        }

        public void Outl (uint val)
        {
            DWord = val;
        }

        public static void Outl (ushort addr, uint val)
        {
            Native.Out32 (addr, val);
        }

        public byte Byte
        {
            get
            {
                return Native.In8 (Address);
            }
            set
            {
                Native.Out8 (Address, value);
            }
        }

        public ushort Word
        {
            get
            {
                return Native.In16 (Address);
            }
            set
            {
                Native.Out16 (Address, value);
            }
        }

        public uint DWord
        {
            get
            {
                return Native.In32 (Address);
            }
            set
            {
                Native.Out32 (Address, value);
            }
        }
    }
}

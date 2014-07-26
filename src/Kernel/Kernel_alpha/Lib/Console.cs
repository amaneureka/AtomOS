using System;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha.Lib
{
    public class ConsoleImpl
    {
        /// <summary>
        /// Console Pointer = x + y*80
        /// </summary>
        public static uint Pointer = 0;
        /// <summary>
        /// Color of Console
        /// </summary>
        public static byte Color = 0xC;
        /// <summary>
        /// Show Cursor
        /// </summary>
        public static bool ShowCursor;

        #region _x86_
        [Plug("System_Void_System_Console_WriteLine_System_String_", CPUArch.x86)]
        public static void WriteLine_x86(string str)
        {
            Write_x86(str);
            Pointer = ((uint)(Pointer / 80) + 1) * 80;
        }

        [Plug("System_Void_System_Console_WriteLine__", CPUArch.x86)]
        public static void WriteLine_x86()
        {
            Pointer = ((uint)(Pointer / 80) + 1) * 80;
        }

        [Plug("System_Void_System_Console_Write_System_String_", CPUArch.x86)]
        public static void Write_x86(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                Console.Write(str[i]);
            }
        }

        [Plug("System_Void_System_Console_Write_System_Char_", CPUArch.x86)]
        public static void Write_x86(char chr)
        {
            unsafe
            {
                byte* xAddress = (byte*)0xB8000;
                xAddress[Pointer * 2] = (byte)(chr);
                xAddress[Pointer * 2 + 1] = Color;
            }
            Pointer++;
            if (Pointer > 25 * 80)
            {
                ScrollUP();
            }
            UpdatePosition();
        }
        #endregion

        [Plug("System_Void_System_Console_Clear__")]
        public static void Clear()
        {
            Pointer = 0;
            for (int i = 0; i < 80 * 25; i++)
                Console.Write(' ');
            Pointer = 0;
        }

        public static unsafe void ScrollUP()
        {
            byte* xAddress = (byte*)0xB8000;
            for (int j = 0; j < 24; j++)
            {
                for (int i = 0; i < 80; i++)
                {
                    xAddress[(j * 80 + i) * 2] = xAddress[((j + 1) * 80 + i) * 2];
                    xAddress[(j * 80 + i) * 2 + 1] = xAddress[((j + 1) * 80 + i) * 2 + 1];
                }
            }
            for (int i = 0; i < 80; i++)
            {
                xAddress[(24 * 80 + i) * 2] = (byte)' ';
                xAddress[(24 * 80 + i) * 2 + 1] = (byte)Color;
            }
            Pointer = 24 * 80;
        }

        /// <summary>
        /// Update pointer position on screen
        /// </summary>
        private static void UpdatePosition()
        {
            if (!ShowCursor)
            {
                // Cursor low byte to VGA index register                
                Native.Out8(0x03D4, 0x0F);
                Native.Out8(0x03D5, (byte)(0xFF1 & 0xFF));
                // Cursor high byte to VGA index register
                Native.Out8(0x03D4, 0x0E);
                Native.Out8(0x03D5, (byte)((0xFF1 >> 8)));
            }
            else
            {
                char xPos = (char)Pointer;
                // Cursor low byte to VGA index register
                Native.Out8(0x03D4, 0x0F);
                Native.Out8(0x03D5, (byte)(xPos & 0xFF));
                // Cursor high byte to VGA index register
                Native.Out8(0x03D4, 0x0E);
                Native.Out8(0x03D5, (byte)((xPos >> 8)));
            }
        }
    }
}

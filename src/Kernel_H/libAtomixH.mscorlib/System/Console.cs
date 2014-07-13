using System;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

using libAtomixH.IO.Ports;

namespace libAtomixH.mscorlib.System
{
    public static unsafe class Console
    {
        private static int X = 0;
        private static int Y = 0;

        #region Plugs
        [Plug("System_Void_System_Console_set_CursorLeft_System_Int32_")]
        private static void CursorLeft(int value)
        {
            X = value;
        }
        [Plug("System_Int32_System_Console_get_CursorLeft__")]
        private static int CursorLeft()
        { 
            return X; 
        }

        [Plug("System_Void_System_Console_set_CursorTop_System_Int32_")]
        private static void CursorTop(int value)
        {
            Y = value;
        }

        [Plug("System_Int32_System_Console_get_CursorTop__")]
        private static int CursorTop()
        {
            return Y;
        }
        [Plug("System_Void_System_Console_set_ForegroundColor_System_ConsoleColor_")]
        private static void ForColor(ConsoleColor value)
        {
            ForegroundColor = value;
        }
        [Plug("System_ConsoleColor_System_Console_get_ForegroundColor__")]
        private static ConsoleColor ForColor()
        {
            return ForegroundColor;
        }

        [Plug("System_Void_System_Console_set_BackgroundColor_System_ConsoleColor_")]
        private static void BackColor(ConsoleColor value)
        {
            BackgroundColor = value;
        }

        [Plug("System_ConsoleColor_System_Console_get_BackgroundColor__")]
        private static ConsoleColor BackColor()
        {
            return BackgroundColor;
        }
        #endregion
        private static ConsoleColor ForegroundColor = ConsoleColor.White;
        private static ConsoleColor BackgroundColor = ConsoleColor.Cyan;

        private static bool IsClearing = false;

        [Plug ("System_Void_System_Console_Clear__", CPUArch.x86)]
        public static void Clear ()
        {
            X = 0;
            Y = 0;
            IsClearing = true;

            byte* offset = (byte*)0xB8000;
            for (int i = 0; i < (80 * 25); i++)
            {
                Write (" ");
            }

            IsClearing = false;
            X = 0;
            Y = 0;

            UpdateCursor ();
        }

        [Plug ("System_Void_System_Console_Write_System_Char_", CPUArch.x86)]
        public static void Write (char chr)
        {
            if (!IsClearing)
                CheckOverflow ();

            byte* offset = (byte*)0xB8000;

            offset[(X + (Y * 80)) * 2] = (byte)chr;
            offset[((X + (Y * 80)) * 2) + 1] = CalculateColor ();

            X++;
        }

        [Plug ("System_Void_System_Console_Write_System_String_", CPUArch.x86)]
        public static void Write (string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                Write (str[i]);
            }
        }

        [Plug ("System_Void_System_Console_WriteLine__", CPUArch.x86)]
        public static void WriteLine ()
        {
            Y++;
            CheckOverflow ();
        }

        [Plug ("System_Void_System_Console_WriteLine_System_Char_", CPUArch.x86)]
        public static void WriteLine (char chr)
        {
            Write (chr);
            Newline ();
        }

        [Plug ("System_Void_System_Console_WriteLine_System_String_", CPUArch.x86)]
        public static void WriteLine (string str)
        {
            Write (str);
            Newline ();
        }

        [Plug ("System_Void_System_Console_SetCursorPosition_System_Int32__System_Int32_", CPUArch.x86)]
        public static void SetCursorPosition (int x, int y)
        {
            if (x <= 80 && y <= 25)
            {
                X = x;
                Y = y;
            }
            UpdateCursor ();
        }

        private static void Newline ()
        {
            X = 0;
            Y++;
            CheckOverflow ();
        }

        private static byte CalculateColor ()
        {
            byte color = (byte)(((byte)BackgroundColor << 4) | (byte)ForegroundColor);
            return color;
        }

        private static void CheckOverflow ()
        {
            bool update = false;

            if (X == 80)
            {
                X = 0;
                Y++;
                update = true;
            }
            if (Y > 80)
            {
                ScrollUp ();
                Y = 80;
                update = true;
            }

            UpdateCursor ();
        }

        private static void UpdateCursor ()
        {
            char chr = (char)(X + (Y * 80));

            // Cursor low byte to VGA index register
            Native.Out8 (0x03D4, 0x0F);
            Native.Out8 (0x03D5, (byte)(chr & 0xFF));

            // Cursor high byte to VGA index register
            Native.Out8 (0x03D4, 0x0E);
            Native.Out8 (0x03D5, (byte)((chr >> 8)));
        }

        private static void ScrollUp ()
        {
            byte* offset = (byte*)0xB8000;

            byte[] bytes = new byte[80 * 24];
            for (int i = 80; i < 80 * 24; i++)
            {
                bytes[i - 80] = offset[i];
            }

            for (int i = 0; i < 80 * 24; i++)
            {
                offset[i] = bytes[i];
            }
            for (int i = 80 * 24; i < 80; i++)
            {
                offset[(i + (25 * 80)) * 2] = (byte)' ';
                offset[((X + (Y * 80)) * 2) + 1] = CalculateColor ();
            }
        }
    }
}

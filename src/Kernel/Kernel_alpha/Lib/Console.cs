using System;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Kernel_alpha.x86.Intrinsic;
using Kernel_alpha.Drivers.Input;

namespace Kernel_alpha.Lib
{
    public class ConsoleImpl
    {

        private static bool verbose = false;

        private static int X = 0;
        private static int Y = 0;

        private static ConsoleColor ForegroundColor = ConsoleColor.White;
        private static ConsoleColor BackgroundColor = ConsoleColor.Black;

        private static int Width = 80;
        private static int Height = 25;

        private static bool IsClearing = false;

        [Plug("System_Void_System_Console_set_WindowWidth_System_Int32_")]
        private static void WindowWidth(int value)
        {
            Width = value;
        }

        [Plug("System_Int32_System_Console_get_WindowWidth__")]
        private static int WindowWidth()
        {
            return Width;
        }

        [Plug("System_Void_System_Console_set_WindowHeight_System_Int32_")]
        private static void WindowHeight(int value)
        {
            Height = value;
        }

        [Plug("System_Int32_System_Console_get_WindowHeight__")]
        private static int WindowHeight()
        {
            return Height;
        }

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
        private static void ForeColor(ConsoleColor value)
        {
            ForegroundColor = value;
        }
        [Plug("System_ConsoleColor_System_Console_get_ForegroundColor__")]
        private static ConsoleColor ForeColor()
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

        [Plug("System_Void_System_Console_Clear__", CPUArch.x86)]
        public static unsafe void Clear()
        {
            X = 0;
            Y = 0;
            IsClearing = true;

            byte* offset = (byte*)0xB8000;
            for (int i = 0; i < (80 * 25); i++)
            {
                Write(" ");
            }

            IsClearing = false;
            X = 0;
            Y = 0;

            UpdateCursor();
        }

        [Plug("System_Void_System_Console_Write_System_Char_", CPUArch.x86)]
        public static unsafe void Write(char chr)
        {
            if (!IsClearing)
                CheckOverflow();

            byte* offset = (byte*)0xB8000;

            offset[(X + (Y * 80)) * 2] = (byte)chr;
            offset[((X + (Y * 80)) * 2) + 1] = CalculateColor();

            X++;
        }

        [Plug("System_Void_System_Console_Write_System_String_", CPUArch.x86)]
        public static void Write(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                char chr = str[i];
                switch (chr)
                {
                    // Bell (alert)
                    case '\a':
                        break;
                    // Backspace
                    case '\b':
                        break;
                    // Formfeed
                    case '\f':
                        break;
                    // Newline
                    case '\n':
                        Newline();
                        break;
                    // Carriage return
                    case '\r':
                        SetCursorPosition(0, Y);
                        break;
                    // Horizontal tab
                    case '\t':
                        break;
                    // Vertical tab
                    case '\v':
                        break;
                    // Single quotation mark
                    case '\'':
                        Write("'");
                        break;
                    // Double quotation mark
                    case '\"':
                        Write('"');
                        break;
                    // Any other characters
                    default:
                        Write(chr);
                        break;
                }
            }
        }

        [Plug("System_Void_System_Console_WriteLine__", CPUArch.x86)]
        public static void WriteLine()
        {
            Newline();
            CheckOverflow();
        }

        [Plug("System_Void_System_Console_WriteLine_System_Char_", CPUArch.x86)]
        public static void WriteLine(char chr)
        {
            Write(chr);
            Newline();
        }

        [Plug("System_Void_System_Console_WriteLine_System_String_", CPUArch.x86)]
        public static void WriteLine(string str)
        {
            Write(str);
            Newline();
        }

        [Plug("System_Void_System_Console_SetCursorPosition_System_Int32__System_Int32_", CPUArch.x86)]
        public static void SetCursorPosition(int x, int y)
        {
            if (x <= 80 && y <= 25)
            {
                X = x;
                Y = y;
            }
            UpdateCursor();
        }

        [Plug("System_Int32_System_Console_Read__", CPUArch.x86)]
        public static int Read()
        {
            return (int)Global.KBD.ReadKey().Char;
        }

        [Plug("System_String_System_Console_ReadLine__", CPUArch.x86)]
        public static string ReadLine()
        {
            StringBuilder sb = new StringBuilder();

            int start = 0;
            int current = 0;
            int max = 0;

            Keys key = null;
            while (key.Code != KeyCode.Enter)
            {
                key = Global.KBD.ReadKey();

                if (key != null)
                {
                    switch (key.Code)
                    {
                        // Handle return
                        case KeyCode.Enter:
                            break;

                        // Handle backspace
                        case KeyCode.Backspace:
                            if (current > start && current <= max)
                            {
                                sb.RemoveAt(current);
                                current--;
                                max--;
                                SetCursorPosition(X - 1, Y);
                                Write(" ");
                                SetCursorPosition(X - 1, Y);
                            }
                            else if (current == start)
                            {
                                sb.Clear();
                            }
                            else if (current == max)
                            {
                                sb.RemoveAt(max);
                                current--;
                                max--;
                                SetCursorPosition(X - 1, Y);
                                Write(" ");
                                SetCursorPosition(X - 1, Y);
                            }
                            break;

                        // Handle the left arrow key
                        case KeyCode.CursorLeft:
                            if (current > start)
                            {
                                current--;
                                SetCursorPosition(X - 1, Y);
                            }
                            break;

                        // Handle the right arrow key
                        case KeyCode.CursorRight:
                            if (current < max)
                            {
                                current++;
                                SetCursorPosition(X + 1, Y);
                            }
                            break;

                        // Handle default
                        default:
                            if (max == 0)
                            {
                                sb.Append(key.Char);
                                Write(key.Char);
                                max++;
                                current++;
                            }
                            else if (current == start)
                            {
                                sb.Append(key.Char);
                                current++;
                                Write(key.Char);
                            }
                            else if (current == max)
                            {
                                sb.Append(key.Char);
                                max++;
                                current++;
                                Write(key.Char);
                            }
                            else if (current < start)
                            {
                                current = start;
                            }
                            else if (current > max)
                            {
                                current = max;
                            }
                            else if (current >= start && current < max)
                            {
                                sb.UpdateAt(current, key.Char);
                                current++;
                                Write(key.Char);
                            }
                            break;
                    }

                    // START: Debug information
                    if (verbose)
                    {
                        int x = X;
                        int y = Y;
                        SetCursorPosition(2, 0);
                        for (int i = 0; i < 77; i++)
                            Write(" ");
                        SetCursorPosition(2, 0);
                        Write("current: " + current.ToString() +
                            " start: " + start.ToString() +
                            " max: " + max.ToString() +
                            " content: " + sb.Flush());
                        SetCursorPosition(x, y);
                    }
                    // END: Debug information

                }
            }

            WriteLine();

            return sb.Flush();
        }

        private static void Newline()
        {
            X = 0;
            Y++;
            CheckOverflow();
        }

        private static byte CalculateColor()
        {
            byte color = (byte)(((byte)BackgroundColor << 4) | (byte)ForegroundColor);
            return color;
        }

        private static void CheckOverflow()
        {
            if (X < 0)
            {
                X = Width + X;
                Y--;
            }
            else if (X == Width)
            {
                X = 0;
                Y++;
            }
            else if (X > Width)
            {
                X = X - Width;
                Y++;
            }

            if (Y < 0)
            {
                Y = 0;
            }
            else if (Y > Height)
            {
                ScrollUp();
                Y = Height;
            }

            UpdateCursor();
        }

        private static void UpdateCursor()
        {
            char chr = (char)(X + (Y * 80));

            // Cursor low byte to VGA index register
            Native.Out8(0x03D4, 0x0F);
            Native.Out8(0x03D5, (byte)(chr & 0xFF));

            // Cursor high byte to VGA index register
            Native.Out8(0x03D4, 0x0E);
            Native.Out8(0x03D5, (byte)((chr >> 8)));
        }

        private static unsafe void ScrollUp()
        {
            int x = X;
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
                xAddress[(24 * 80 + i) * 2 + 1] = (byte)CalculateColor();
            }

            SetCursorPosition(0, 24);
        }
    }
}

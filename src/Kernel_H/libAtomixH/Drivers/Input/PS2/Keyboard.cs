using System;
using System.Collections.Generic;

using libAtomixH.IO.Ports;

namespace libAtomixH.Drivers.Input.PS2
{
    public class Keyboard
    {
        private Queue<Keys> Buffer;
        private int MaxBuffer;

        public bool NumLock;
        public bool CapsLock;
        public bool ScrollLock;

        public bool Shift;
        public bool Alt;
        public bool Ctrl;

        public IOPort commandPort;

        private KeyCode[] Set = new KeyCode[]
        {
            KeyCode.None,           KeyCode.Esc,            KeyCode.D1,             KeyCode.D2,             //0x00
            KeyCode.D3,             KeyCode.D4,             KeyCode.D5,             KeyCode.D6,             //0x04
            KeyCode.D7,             KeyCode.D8,             KeyCode.D9,             KeyCode.D0,             //0x08
            KeyCode.Dash,           KeyCode.Equals,         KeyCode.Backspace,      KeyCode.Tab,            //0x0C
            KeyCode.Q,              KeyCode.W,              KeyCode.E,              KeyCode.R,              //0x10
            KeyCode.T,              KeyCode.Y,              KeyCode.U,              KeyCode.I,              //0x14
            KeyCode.O,              KeyCode.P,              KeyCode.OpenBracket,    KeyCode.CloseBracket,   //0x18
            KeyCode.Enter,          KeyCode.LCtrl,          KeyCode.A,              KeyCode.S,              //0x1C
            KeyCode.D,              KeyCode.F,              KeyCode.G,              KeyCode.H,              //0x20
            KeyCode.J,              KeyCode.K,              KeyCode.L,              KeyCode.SemiColon,      //0x24
            KeyCode.Singlequote,    KeyCode.Backtick,       KeyCode.LShift,         KeyCode.Backslash,      //0x28
            KeyCode.Z,              KeyCode.X,              KeyCode.C,              KeyCode.V,              //0x2C
            KeyCode.B,              KeyCode.N,              KeyCode.M,              KeyCode.Comma,          //0x30
            KeyCode.FullStop,       KeyCode.ForwardSlash,   KeyCode.RShift,         KeyCode.NumMultiply,    //0x34
            KeyCode.LAlt,           KeyCode.Space,          KeyCode.Caps,           KeyCode.F1,             //0x38
            KeyCode.F2,             KeyCode.F3,             KeyCode.F4,             KeyCode.F5,             //0x3C
            KeyCode.F6,             KeyCode.F7,             KeyCode.F8,             KeyCode.F9,             //0x40
            KeyCode.F10,            KeyCode.Num,            KeyCode.Scroll,         KeyCode.Num7,           //0x44
            KeyCode.Num8,           KeyCode.Num9,           KeyCode.NumSubtract,    KeyCode.Num4,           //0x48
            KeyCode.Num5,           KeyCode.Num6,           KeyCode.NumAdd,         KeyCode.Num1,           //0x4C
            KeyCode.Num2,           KeyCode.Num3,           KeyCode.Num0,           KeyCode.NumDecimal,     //0x50
            KeyCode.None,           KeyCode.None,           KeyCode.None,           KeyCode.F11,            //0x54
            KeyCode.F12,

        };

        private KeyCode[] extSet = new KeyCode[]
        {
            KeyCode.None,           KeyCode.None,           KeyCode.None,           KeyCode.None,           //0x00
            KeyCode.None,           KeyCode.None,           KeyCode.None,           KeyCode.None,           //0x04
            KeyCode.None,           KeyCode.None,           KeyCode.None,           KeyCode.None,           //0x08
            KeyCode.None,           KeyCode.None,           KeyCode.None,           KeyCode.None,           //0x0C
            KeyCode.PrevTrack,      KeyCode.None,           KeyCode.None,           KeyCode.None,           //0x10
            KeyCode.None,           KeyCode.None,           KeyCode.None,           KeyCode.None,           //0x14
            KeyCode.None,           KeyCode.NextTrack,      KeyCode.None,           KeyCode.None,           //0x18
            KeyCode.NumEnter,       KeyCode.RCtrl,          KeyCode.None,           KeyCode.None,           //0x1C
            KeyCode.Mute,           KeyCode.Calculator,     KeyCode.Play,           KeyCode.None,           //0x20
            KeyCode.Stop,           KeyCode.None,           KeyCode.None,           KeyCode.None,           //0x24
            KeyCode.None,           KeyCode.None,           KeyCode.None,           KeyCode.None,           //0x28
            KeyCode.None,           KeyCode.None,           KeyCode.VolumeDown,     KeyCode.None,           //0x2C
            KeyCode.VolumeUp,       KeyCode.None,           KeyCode.WWWHome,        KeyCode.None,           //0x30
            KeyCode.None,           KeyCode.NumDivide,      KeyCode.None,           KeyCode.None,           //0x34
            KeyCode.RAlt,           KeyCode.None,           KeyCode.None,           KeyCode.None,           //0x38
            KeyCode.None,           KeyCode.None,           KeyCode.None,           KeyCode.None,           //0x3C
            KeyCode.None,           KeyCode.None,           KeyCode.None,           KeyCode.None,           //0x40            
            KeyCode.None,           KeyCode.None,           KeyCode.None,           KeyCode.Home,           //0x44
            KeyCode.CursorUp,       KeyCode.PageUP,         KeyCode.NumSubtract,    KeyCode.CursorLeft,     //0x48
            KeyCode.None,           KeyCode.CursorRight,    KeyCode.NumAdd,         KeyCode.End,            //0x4C
            KeyCode.CursorDown,     KeyCode.PageDown,       KeyCode.Insert,         KeyCode.Delete,         //0x50
            KeyCode.None,           KeyCode.None,           KeyCode.None,           KeyCode.None,           //0x54   
            KeyCode.None,           KeyCode.None,           KeyCode.None,           KeyCode.LGUI,           //0x58
            KeyCode.RGUI,           KeyCode.Apps,           KeyCode.None,           KeyCode.None,           //0x5C
            KeyCode.None,           KeyCode.None,           KeyCode.None,           KeyCode.None,           //0x60
            KeyCode.None,           KeyCode.WWWSearch,      KeyCode.WWWFavorites,   KeyCode.WWWRefresh,     //0x64
            KeyCode.WWWStop,        KeyCode.WWWForward,     KeyCode.WWWBack,        KeyCode.MyComputer,     //0x68
            KeyCode.Email,          KeyCode.MediaSelect,    KeyCode.None,           KeyCode.None,           //0x6C
        };

        public Keyboard ()
        {
            // Set the command port
            commandPort = new IOPort ((ushort)PS2Ports.ps2Port.PS2_Cmd);

            // Set Buffer size and init buffer
            MaxBuffer = 100;
            Buffer = new Queue<Keys> (MaxBuffer);

            // Set LEDs
            UpdateLEDs ();
        }

        private bool IsExtended = false;
        public void HandleIRQ ()
        {
            uint xScanCode = commandPort.Byte;

            if (xScanCode == 0xE0)
            {
                IsExtended = true; //The ScanCode is extended
                return;
            }

            //Set break
            bool xReleased = (xScanCode & 0x80) == 0x80;
            if (xReleased)
            {
                xScanCode = (uint)(xScanCode ^ 0x80);
            }

            KeyCode xKey = KeyCode.None;

            if (IsExtended || (NumLock && (xScanCode >= 0x47 && xScanCode <= 0x53)))
                xKey = extSet[xScanCode];
            else
                xKey = Set[xScanCode];

            switch (xKey)
            {
                case KeyCode.None: //We will not check any code which we don't know
                    break;
                case KeyCode.LShift:
                case KeyCode.RShift:
                    Shift = !xReleased;
                    break;
                case KeyCode.LCtrl:
                    Ctrl = !xReleased;
                    break;
                case KeyCode.LAlt:
                    Alt = !xReleased;
                    break;
                default:
                    {
                        if (!xReleased)
                        {
                            bool UpdateLED = false;
                            switch (xKey)
                            {
                                case KeyCode.Caps:
                                    CapsLock = !CapsLock;
                                    UpdateLED = true;
                                    break;
                                case KeyCode.Num:
                                    NumLock = !NumLock;
                                    UpdateLED = true;
                                    break;
                                case KeyCode.Scroll:
                                    ScrollLock = !ScrollLock;
                                    UpdateLED = true;
                                    break;
                                default:
                                    {
                                        Buffer.Enqueue (new Keys (xKey, GetKeyChar (xKey)));
                                    }
                                    break;
                            }

                            if (UpdateLED)
                                UpdateLEDs ();
                        }
                    }
                    break;
            }
            IsExtended = false;
        }

        private void UpdateLEDs ()
        {
            byte Value = (byte)((byte)(CapsLock ? 0x4 : 0x0) |
                                (byte)(NumLock ? 0x2 : 0x0) |
                                (byte)(ScrollLock ? 0x1 : 0x0));
            PS2Ports.SendCommand (PS2Ports.ps2Cmd.Key_LEDs, Value);
        }

        private char GetKeyChar (KeyCode aKey)
        {
            bool Case = (CapsLock && !Shift) ? true : ((!CapsLock && Shift) ? true : false);
            switch (aKey)
            {
                #region Alphabets
                case KeyCode.A: return Case ? 'A' : 'a';
                case KeyCode.B: return Case ? 'B' : 'b';
                case KeyCode.C: return Case ? 'C' : 'c';
                case KeyCode.D: return Case ? 'D' : 'd';
                case KeyCode.E: return Case ? 'E' : 'e';
                case KeyCode.F: return Case ? 'F' : 'f';
                case KeyCode.G: return Case ? 'G' : 'g';
                case KeyCode.H: return Case ? 'H' : 'h';
                case KeyCode.I: return Case ? 'I' : 'i';
                case KeyCode.J: return Case ? 'J' : 'j';
                case KeyCode.K: return Case ? 'K' : 'k';
                case KeyCode.L: return Case ? 'L' : 'l';
                case KeyCode.M: return Case ? 'M' : 'm';
                case KeyCode.N: return Case ? 'N' : 'n';
                case KeyCode.O: return Case ? 'O' : 'o';
                case KeyCode.P: return Case ? 'P' : 'p';
                case KeyCode.Q: return Case ? 'Q' : 'q';
                case KeyCode.R: return Case ? 'R' : 'r';
                case KeyCode.S: return Case ? 'S' : 's';
                case KeyCode.T: return Case ? 'T' : 't';
                case KeyCode.U: return Case ? 'U' : 'u';
                case KeyCode.V: return Case ? 'V' : 'v';
                case KeyCode.W: return Case ? 'W' : 'w';
                case KeyCode.X: return Case ? 'X' : 'x';
                case KeyCode.Y: return Case ? 'Y' : 'y';
                case KeyCode.Z: return Case ? 'Z' : 'z';
                #endregion
                case KeyCode.D1: return (Shift) ? '!' : '1';
                case KeyCode.D2: return (Shift) ? '@' : '2';
                case KeyCode.D3: return (Shift) ? '#' : '3';
                case KeyCode.D4: return (Shift) ? '$' : '4';
                case KeyCode.D5: return (Shift) ? '%' : '5';
                case KeyCode.D6: return (Shift) ? '^' : '6';
                case KeyCode.D7: return (Shift) ? '&' : '7';
                case KeyCode.D8: return (Shift) ? '*' : '8';
                case KeyCode.D9: return (Shift) ? '(' : '9';
                case KeyCode.D0: return (Shift) ? ')' : '0';

                case KeyCode.Num0: return '0';
                case KeyCode.Num1: return '1';
                case KeyCode.Num2: return '2';
                case KeyCode.Num3: return '3';
                case KeyCode.Num4: return '4';
                case KeyCode.Num5: return '5';
                case KeyCode.Num6: return '6';
                case KeyCode.Num7: return '7';
                case KeyCode.Num8: return '8';
                case KeyCode.Num9: return '9';
                case KeyCode.NumEnter: return '\n';
                case KeyCode.NumAdd: return '+';
                case KeyCode.NumSubtract: return '-';
                case KeyCode.NumDivide: return '/';
                case KeyCode.NumMultiply: return '*';
                case KeyCode.NumDecimal: return '.';

                case KeyCode.Enter: return '\n';
                case KeyCode.Dash: return (Shift) ? '_' : '-';
                case KeyCode.Equals: return (Shift) ? '+' : '=';
                case KeyCode.OpenBracket: return (Shift) ? '{' : '[';
                case KeyCode.CloseBracket: return (Shift) ? '}' : ']';
                case KeyCode.Backslash: return (Shift) ? '|' : '\\';
                case KeyCode.SemiColon: return (Shift) ? ':' : ';';
                case KeyCode.Singlequote: return (Shift) ? '"' : '\'';
                case KeyCode.Comma: return (Shift) ? '<' : ',';
                case KeyCode.FullStop: return (Shift) ? '>' : '.';
                case KeyCode.ForwardSlash: return (Shift) ? '?' : '/';
                case KeyCode.Backtick: return (Shift) ? '~' : '`';

                case KeyCode.Backspace: return '\b';
                case KeyCode.Space: return ' ';
                default: return '\0';
            }
        }


        public Keys ReadKey ()
        {
            if (Buffer.Count == 0)
                return null;

            return Buffer.Dequeue ();
        }
    }
}

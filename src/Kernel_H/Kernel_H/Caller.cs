using System;

namespace Kernel_H
{
    public static class Caller
    {
        public static unsafe void Start ()
        {
            // Do the initialization stuff here
            // For now, let's just print something
            WriteScreen ("AtomixOS H booted successfully!", 0);

            while (true)
            {
                Update ();
            }
        }

        public static void Update ()
        {

        }

        public static unsafe void WriteScreen (string s, int p)
        {
            byte* xA = (byte*)0xB8000;
            for (int i = 0; i < s.Length; i++)
            {
                xA[p++] = (byte)s[i];
                xA[p++] = 0x0B;
            }
        }
    }
}

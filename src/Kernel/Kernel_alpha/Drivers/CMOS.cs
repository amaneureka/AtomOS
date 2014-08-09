using System;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha.Drivers
{
    public static class CMOS
    {
        #region enum
        enum cmos : byte
        {
            Address = 0x70,
            Data = 0x71
        }

        enum Offset : byte
        {
            RTC_Second = 0,
            RTC_Minute = 2,
            RTC_Hour = 4,
            RTC_DayOfWeek = 6,
            RTC_Day = 7,
            RTC_Month = 8,
            RTC_Year = 9,
            RTC_CenturyDay = 50,
            RTC_Century = 72
        }
        #endregion

        public static uint Seconds
        {
            get
            {
                WaitForReady();
                return FromBCD(RTC_Register((byte)Offset.RTC_Second));
            }
        }

        public static uint Minutes
        {
            get
            {
                WaitForReady();
                return FromBCD(RTC_Register((byte)Offset.RTC_Minute));
            }
        }

        public static uint Hours
        {
            get
            {
                WaitForReady();
                return FromBCD(RTC_Register((byte)Offset.RTC_Hour));
            }
        }

        public static uint Day
        {
            get
            {
                WaitForReady();
                return FromBCD(RTC_Register((byte)Offset.RTC_Day));
            }
        }

        public static uint DayOfWeek
        {
            get
            {
                WaitForReady();
                return FromBCD(RTC_Register((byte)Offset.RTC_DayOfWeek));
            }
        }

        public static uint Month
        {
            get
            {
                WaitForReady();
                return FromBCD(RTC_Register((byte)Offset.RTC_Month));
            }
        }

        public static uint Year
        {
            get
            {
                WaitForReady();
                return FromBCD(RTC_Register((byte)Offset.RTC_Year));
            }
        }

        public static uint CenturyDay
        {
            get
            {
                WaitForReady();
                return FromBCD(RTC_Register((byte)Offset.RTC_CenturyDay));
            }
        }

        public static uint Century
        {
            get
            {
                WaitForReady();
                return FromBCD(RTC_Register((byte)Offset.RTC_Century));
            }
        }

        private static uint FromBCD(uint value)
        {
            return (uint)(((value >> 4) & 0x0F) * 10 + (value & 0x0F));
        }

        private static byte RTC_Register(byte aNo)
        {
            IOPort.Outb((byte)cmos.Address, aNo);
            return IOPort.Inb((byte)cmos.Data);
        }

        private static void WaitForReady()
        {
            do
            {
                IOPort.Outb((byte)cmos.Address, 10);
            }
            while ((IOPort.Inb((byte)cmos.Data) & 0x80) != 0);
        }
    }
}

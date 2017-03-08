/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          FAT Helper
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO.FileSystem.FAT.Find
{
    internal class WithName : Comparison
    {
        internal string Name;

        internal WithName(string aName)
        {
            Name = aName;
        }

        internal override bool Compare(byte[] data, int offset, FatType type)
        {
            switch ((FileNameAttribute)data[offset + (int)Entry.DOSName])
            {
                case FileNameAttribute.LastEntry:
                case FileNameAttribute.Deleted:
                case FileNameAttribute.Escape:
                    return false;
                default:
                    break;
            }

            int index;
            for (index = 7; index >= 0 && data[offset + index] == ' '; index--) ;
            index++;

            int dot = Name.IndexOf('.');
            if (dot == -1)
                dot = Name.Length;

            if (dot != index)
                return false;

            for (index = 0; index < dot; index++)
            {
                if ((data[offset + index] & 0xDF) != (Name[index] & 0xDF))
                    return false;
            }

            for (index = 10; index >= 8 && data[offset + index] == ' '; index--) ;
            index -= 7;

            int index2 = Name.Length - dot - 1;
            if (index2 < 0) index2 = 0;

            if (index2 != index)
                return false;

            dot++;
            for (index = 0; index < index2; index++)
            {
                if ((data[offset + index + 8] & 0xDF) != (Name[dot + index] & 0xDF))
                    return false;
            }

            return true;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Atomix.Kernel_H.io.FileSystem.FAT.Find
{
    public class WithName : Comparison
    {
        public string Name;

        public WithName(string aName)
        {
            this.Name = aName;
        }

        public override bool Compare(byte[] data, int offset, FatType type)
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
                if (data[offset + index] != Name[index])
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
                if (data[offset + index + 8] != Name[dot + index])
                    return false;
            }

            return true;
        }
    }
}

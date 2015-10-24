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
#warning Sandeep: Reply on below comment ASAP
                    //case FileNameAttribute.Dot:
                    return false;
                default:
                    break;
            }

#warning Not well qualified
            for (int i = 0; i < Name.Length; i++)
            {
                if (Name[i] == '.')
                    return true;
                if (data[offset + i] != Name[i])
                    return false;
            }
            return true;
        }
    }
}

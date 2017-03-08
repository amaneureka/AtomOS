/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:
* PROGRAMMERS:      SANDEEP ILIGER <sandeep.iliger@gmail.com>
*                   Aman Priyadarshi <aman.eureka@gmail.com>
*/

using Kernel_alpha.Lib;
using Kernel_alpha.Lib.Encoding;
using Kernel_alpha.FileSystem.FAT;

namespace Kernel_alpha.FileSystem.Find
{
	public class WithName : ACompare
	{
		protected string name;

		public WithName(string name)
		{
			this.name = name;
		}

		public override bool Compare(byte[] data, uint offset, FatType type)
		{
            BinaryFormat entry = new BinaryFormat(data);

            byte first = entry.GetByte(offset + Entry.DOSName);

            if (first == FileNameAttribute.LastEntry)
                return false;

            if ((first == FileNameAttribute.Deleted)) //| (first == FileSystem.FAT.FatFileSystem.FileNameAttribute.Dot)
                return false;

            if (first == FileNameAttribute.Escape)
                return false;

            string entryname = ASCII.GetString(data, (int)offset, 8).Trim();
            string entryExt = ASCII.GetString(data, (int)(offset + 8), 3).Trim();

            string[] xStr = name.Split('.');
            if (xStr.Length > 1)
            {
                if (entryname.ToLower() == xStr[0].Trim('\0').ToLower() && entryExt.ToLower() == xStr[1].Trim('\0').ToLower())
                {
                    return true;
                }
            }

            if (entryname.ToLower() == this.name.Trim().ToLower())
            {
                return true;
            }

			return false;
		}
	}
}
/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:
* PROGRAMMERS:      SANDEEP ILIGER <sandeep.iliger@gmail.com>
*                   Aman Priyadarshi <aman.eureka@gmail.com>
*/

using Kernel_alpha.Lib;
using Kernel_alpha.FileSystem.FAT;

namespace Kernel_alpha.FileSystem.Find
{
	public class Empty : ACompare
	{
		protected uint cluster;

		public Empty()
		{
		}

		public override bool Compare(byte[] data, uint offset, FatType type)
		{
			BinaryFormat entry = new BinaryFormat(data);

			byte first = entry.GetByte(offset + Entry.DOSName);

            if (first == FileNameAttribute.LastEntry)
				return true;

            //if ((first == FileSystem.FAT.FatFileSystem.FileNameAttribute.Deleted) | (first == FileSystem.FAT.FatFileSystem.FileNameAttribute.Dot))
            //    return true;

			return false;
		}
	}
}
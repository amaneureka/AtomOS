/* Copyright (C) Atomix OS Development, Inc - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by SANDEEP ILIGER <sandeep.iliger@gmail.com>, 08-2014
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
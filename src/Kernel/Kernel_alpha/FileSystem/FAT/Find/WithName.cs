/* Copyright (C) Atomix OS Development, Inc - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by SANDEEP ILIGER <sandeep.iliger@gmail.com>, 08-2014
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

            // Have to modify some things -- SANDEEP

            //	string entryname = FatFileSystem.ExtractFileName(data, offset);
            string entryname = ASCII.GetString(data, (int)offset, 8).Trim();
            string entryExt = ASCII.GetString(data, (int)(offset + 8), 3).Trim();


            //string[] names;
            //if (name == ".." || name == ".")
            //    names = name.Split(' ');
            //else
            //    names = name.Split('.');

            ////Array.Resize(ref names, 2);  have to change SANDEEP
            //if (names[1] == null)
            //    names[1] = "";

            // Only for Directories
            if(entryname.ToLower() == this.name.Trim().ToLower())
            {
                return true;
            }
            //if (entryname.ToLower() == names[0].Trim().ToLower() && entryExt.ToLower() == names[1].Trim().ToLower())
            //{
            //    return true;
            //}
           
			return false;
		}
	}
}
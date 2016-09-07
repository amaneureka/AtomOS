using System;
using System.Collections.Generic;

namespace Kernel_alpha.FileSystem
{
    public static class VirtualFileSystem
    {
        private static List<GenericFileSystem> FileSystem;

        public static void Init()
        {
            FileSystem = new List<GenericFileSystem>();
        }

        public static bool Mount(GenericFileSystem FS)
        {
            //TODO: Will have to do much things in future
            FileSystem.Add(FS);
            return true;
        }

        public static GenericFileSystem GetFSNode(int index)
        {
            if (index >= FileSystem.Count)
                throw new Exception("FileSystem Doest not Exist!");
            return FileSystem[index];
        }

        public static GenericFileSystem GetFSNode(char index)
        {
            var aIndex = (int)(index - 0x61);//Only small case char
            if (aIndex >= FileSystem.Count)
                throw new Exception("FileSystem Doest not Exist!");
            return FileSystem[aIndex];
        }
    }
}

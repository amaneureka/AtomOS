using System;

using Atomix.Kernel_H.lib;

using Atomix.Kernel_H.io.FileSystem.RFS;

namespace Atomix.Kernel_H.io.FileSystem
{
    public class RamFileSystem : GenericFileSystem
    {
        IList<RamFile> Files;

        public RamFileSystem()
        {
            Files = new IList<RamFile>();
        }

        public override Stream GetFile(string[] path, int pointer)
        {
            for (int i = 0; i < Files.Count; i++)
                if (Files[i].Name == path[pointer])
                    return new FileStream(Files[i]);
            return null;
        }

        public override bool CreateFile(string[] path, int pointer)
        {
            Files.Add(new RamFile(path[pointer]));
            return true;
        }
    }
}

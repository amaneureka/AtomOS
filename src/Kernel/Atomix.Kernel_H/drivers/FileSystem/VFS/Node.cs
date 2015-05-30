using System;

namespace Atomix.Kernel_H.drivers.FileSystem.VFS
{
    public class Node
    {
        public string Name;

        public Node(string aName)
        {
            this.Name = aName;
        }
    }
}

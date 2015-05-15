using System;
using System.Collections.Generic;

namespace Atomix.Kernel_H.drivers.FileSystem.VFS
{
    public class Directory : Node
    {
        private List<Node> Childrens;

        public Directory(string aName)
            :base(aName)
        {
            this.Childrens = new List<Node>();
        }

        public bool Add(Node child)
        {
            for (int i = 0; i < Childrens.Count; i++)
                if (Childrens[i].Name == child.Name)//Entry with the same name is not allowed
                    return false;
            Childrens.Add(child);
            return true;
        }

        public bool Remove(string name)
        {
            for (int i = 0; i < Childrens.Count; i++)
            {
                if (Childrens[i].Name == name)
                {
                    Childrens.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }
}

using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.io.FileSystem.VFS
{
    public class Directory : Node
    {
        private IList<Node> Childrens;

        public Directory(string aName)
            :base(aName)
        {
            this.Childrens = new IList<Node>();
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
#warning IMPLEMENT RemoveAt in IList
                    //Childrens.RemoveAt(i);
                    return false;
                }
            }
            return false;
        }

        public Node GetEntry(string name)
        {
            Node curr;
            for (int i = 0; i < Childrens.Count; i++)
            {
                curr = Childrens[i];
                if (curr.Name == name)
                {
                    return curr;
                }
            }
#warning add error feature here
            return null;
        }
    }
}

/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          VFS Directory
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO
{
    internal abstract class Directory : FSObject
    {
        internal Directory(string aName)
            : base(aName)
        {

        }

        internal virtual FSObject FindEntry(string aName)
        {
            return null;
        }

        internal virtual File CreateFile(string aName)
        {
            return null;
        }

        internal virtual Directory CreateDirectory(string aName)
        {
            return null;
        }
    }
}
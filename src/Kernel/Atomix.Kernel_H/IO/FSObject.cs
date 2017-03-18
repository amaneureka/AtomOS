/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          VFS Object
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO
{
    internal abstract class FSObject
    {
        internal readonly string Name;

        internal FSObject(string aName)
        {
            Name = aName;
        }
    }
}
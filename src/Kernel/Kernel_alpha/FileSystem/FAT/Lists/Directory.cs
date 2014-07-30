using System;
using System.Collections.Generic;


namespace Kernel_alpha.FileSystem.FAT.Lists
{
    class Directory : Base
    { 
          public Directory(string aName, string adate) : base( aName) 
          {
              ModifiedDate = adate;
          }

    }
}

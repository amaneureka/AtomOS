/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is strictly prohibited
*                   Proprietary and confidential
* PURPOSE:          Generic Font class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.io;

namespace Atomix.Kernel_H.gui.font
{
    public abstract class GenericFont
    {
        protected string mFontName;
        protected Stream mStream;
        protected bool mIsValid;

        public bool IsValid
        { get { return mIsValid; } }

        public GenericFont(string aFontName, Stream aStream)
        {
            this.mFontName = aFontName;
            this.mStream = aStream;
            this.mIsValid = false;
        }
    }
}

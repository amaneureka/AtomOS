/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Generic Font class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.IO;

namespace Atomix.Kernel_H.Gui.Font
{
    public abstract class GenericFont
    {
        protected string mFontName;
        protected Stream mStream;
        protected bool mIsValid;

        internal bool IsValid
        { get { return mIsValid; } }

        internal GenericFont(string aFontName, Stream aStream)
        {
            mFontName = aFontName;
            mStream = aStream;
            mIsValid = false;
        }
    }
}

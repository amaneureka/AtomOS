using System;

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

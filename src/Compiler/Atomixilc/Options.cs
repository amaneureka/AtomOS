/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Compiler Build Options
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.IO;
using System.Collections.Generic;

namespace Atomixilc
{
    internal class Options
    {
        bool mVerbose = false;
        bool mOptimize = false;
        string mOutputFile = string.Empty;
        List<string> mInputsDlls = null;
        Architecture mTargetPlatform = Architecture.None;

        public List<string> InputFiles
        {
            get
            {
                if (mInputsDlls == null)
                    mInputsDlls = new List<string>();

                return mInputsDlls;
            }
        }

        public string OutputFile
        {
            get { return mOutputFile; }
            set { mOutputFile = value; }
        }

        public Architecture TargetPlatform
        {
            get { return mTargetPlatform; }
            set { mTargetPlatform = value; }
        }

        public bool Verbose
        {
            get { return mVerbose; }
            set { mVerbose = value; }
        }

        public bool Optimize
        {
            get { return mOptimize; }
            set { mOptimize = value; }
        }

        public void Normalize()
        {
            if (InputFiles.Count == 0)
                throw new Exception("No Input File");

            if (string.IsNullOrEmpty(mOutputFile))
                mOutputFile = "App.asm";

            if (TargetPlatform == Architecture.None)
                throw new Exception("No target platform selected");

            if (!Path.IsPathRooted(mOutputFile))
                mOutputFile = Path.Combine(Environment.CurrentDirectory, mOutputFile);

            Directory.CreateDirectory(Path.GetDirectoryName(mOutputFile));
            for (int i = 0; i < InputFiles.Count; i++)
            {
                string InputFile = InputFiles[i];
                if (!Path.IsPathRooted(InputFile))
                    InputFile = Path.Combine(Environment.CurrentDirectory, InputFile);
                if (!File.Exists(InputFile))
                    throw new Exception(string.Format("Input file: '{0}' Does not exist", InputFile));
                InputFiles[i] = InputFile;
            }
        }
    }
}

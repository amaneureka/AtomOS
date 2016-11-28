using System;
using System.Collections.Generic;

namespace Atomixilc
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Options CompilerOptions = null;

                ParseArguments(args, out CompilerOptions);
                CompilerOptions.Normalize();

                new Compiler(CompilerOptions).Execute();
            }
            catch (Exception e)
            {
                Verbose.Error(e.ToString());
            }
        }

        internal static void ParseArguments(string[] args, out Options CompilerOptions)
        {
            Options Options = new Options();

            int index = 0;
            while(index < args.Length)
            {
                string flag = args[index];

                if (flag == "-v") Options.Verbose = true;
                else if (flag == "-optimize") Options.Optimize = true;
                else if (flag == "-cpu")
                {
                    index++;
                    if (index >= args.Length)
                        throw new Exception("Invalid platform parameter");

                    flag = args[index];
                    switch(flag)
                    {
                        case "x86": Options.TargetPlatform = Architecture.x86; break;
                        case "x64": Options.TargetPlatform = Architecture.x64; break;
                        case "ARM": Options.TargetPlatform = Architecture.ARM; break;
                        default: throw new Exception("Invalid target platform");
                    }
                }
                else if (flag == "-i")
                {
                    index++;
                    if (index >= args.Length)
                        throw new Exception("Invalid input files parameter");
                    Options.InputFiles.AddRange(args[index].Split(';'));
                }
                else if (flag == "-o")
                {
                    index++;
                    if (index >= args.Length)
                        throw new Exception("Invalid output files parameter");
                    Options.OutputFile = args[index];
                }

                index++;
            }

            CompilerOptions = Options;
        }
    }
}

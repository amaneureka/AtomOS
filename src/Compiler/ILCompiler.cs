using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Atomix.CompilerExt;

namespace Atomix
{
    public class ILCompiler
    {
        /// <summary>
        /// Kernel Main Dll
        /// </summary>
        public static string InputDll;

        /// <summary>
        /// Other Assembly goes here
        /// </summary>
        public static List<string> InputFiles;

        /// <summary>
        /// Build Output Directory
        /// </summary>
        public static string OutputDir;

        /// <summary>
        /// CPU Architechture i.e. x86, ARM, x64
        /// </summary>
        public static CPUArch CPUArchitecture = CPUArch.none;

        /// <summary>
        /// This logs our every step
        /// </summary>
        public static Logger Logger;
        
        public static void Main(string[] args)
        {
            InputFiles = new List<string>();
            bool DoLogging = false;
            bool DoOptimization = false;
            try
            {
                #region Parsing
                if (args.Length > 0)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] == "-cpu")
                        {
                            switch (args[i + 1])
                            {
                                case "x86":
                                    CPUArchitecture = CPUArch.x86;
                                    break;
                                case "x64":
                                    CPUArchitecture = CPUArch.x64;
                                    break;
                                case "arm":
                                    CPUArchitecture = CPUArch.ARM;
                                    break;
                                default:
                                    CPUArchitecture = CPUArch.none;
                                    break;
                            }
                            i++;
                        }
                        else if (args[i] == "-o")
                        {
                            OutputDir = args[i + 1];
                            i++;
                        }
                        else if (args[i] == "-d")
                        {
                            DoLogging = true;
                        }
                        else if (args[i] == "-i")
                        {
                            var xInput = args[i + 1].Split(';');

                            InputDll = xInput[0];
                            
                            if (!File.Exists(InputDll))
                                throw new Exception("Kernel Assembly not found");

                            for (int j = 1; j < xInput.Length; j++)
                            {
                                if (!File.Exists(xInput[j]))
                                    throw new Exception("Some of binded files does not exist");

                                if (!Path.IsPathRooted(InputDll))
                                    InputFiles.Add(Path.Combine(Environment.CurrentDirectory, xInput[j]));
                                else
                                    InputFiles.Add(xInput[j]);                                
                            }
                            i++;
                        }
                        else if (args[i] == "-optimize")
                        {
                            DoOptimization = true;
                        }
                    }
                
                }
                else
                {
                    throw new Exception("No Input Parameter");
                }
                #endregion

                if (CPUArchitecture == CPUArch.none)
                    throw new Exception("No Output Platform Selected");

                if (InputDll == string.Empty || InputDll == null)
                    throw new Exception("No input kernel assembly");
                else if (!Path.IsPathRooted(InputDll))
                    InputDll = Path.Combine(Environment.CurrentDirectory, InputDll);
                                
                if (OutputDir == string.Empty || OutputDir == null)
                    OutputDir = Environment.CurrentDirectory;
                else
                    OutputDir = Path.Combine(Environment.CurrentDirectory, OutputDir);
                
                /* Building Starts Here */
                Logger = new Logger(OutputDir, DoLogging);
                Logger.Write("@ILCompiler", "Initialized parameters", "Building Started...");                
                Logger.Write("Architecture     : " + CPUArchitecture);
                Logger.Write("Output Directory : " + OutputDir);
                Logger.Write("Input Assembly   : " + InputDll);
                Compiler xCompiler = new Compiler(DoOptimization); 
                try
                {                  
                    xCompiler.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Logger.Dump();
                xCompiler.FlushAsmFile();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }            
        }
    }
}

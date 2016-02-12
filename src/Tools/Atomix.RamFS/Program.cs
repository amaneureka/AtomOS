using System;
using System.IO;
using System.Collections.Generic;

namespace Atomix.RamFS
{
    public class Program
    {
        static string InputFile;
        static string OutputFile;

        public static void Main(string[] args)
        {
            try
            {
                /* load configurations */
                for (int index = 0; index < args.Length; index++)
                {
                    var xCurrent = args[index];
                    if (xCurrent == "-o")
                        OutputFile = args[++index];
                    else
                        InputFile = xCurrent;
                }

                if (!Path.IsPathRooted(InputFile))
                    InputFile = Path.Combine(Environment.CurrentDirectory, InputFile);

                if (!Path.IsPathRooted(OutputFile))
                    OutputFile = Path.Combine(Environment.CurrentDirectory, OutputFile);

                if (OutputFile.EndsWith("\\"))
                    OutputFile = OutputFile.Substring(0, OutputFile.Length - 1);

                if (InputFile.EndsWith("\\"))
                    InputFile = InputFile.Substring(0, InputFile.Length - 1);

                CreateImage();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
        }

        private static void CreateImage()
        {
            if (InputFile == null)
                throw new Exception("Input File not set!");

            if (OutputFile == null)
                throw new Exception("Output File not set!");

            if (!Directory.Exists(InputFile))
                throw new Exception("Input Directory not present!");

            var FilesToAdd = new List<FileEntry>();
            foreach(var FilePath in Directory.EnumerateFiles(InputFile))
                FilesToAdd.Add(new FileEntry(Path.GetFileName(FilePath), File.OpenRead(FilePath)));

            int DataAreaPointer = (FilesToAdd.Count * 32);
            Align256(ref DataAreaPointer);

            int EntryPointer = 32;
            using (var xOutput = File.Create(OutputFile))
            {
                //TODO: First Entry
                var HeaderInfo = new byte[32];
                foreach(var file in FilesToAdd)
                {
                    file.GetEntryData(DataAreaPointer, HeaderInfo);

                    xOutput.Seek(EntryPointer, SeekOrigin.Begin);
                    xOutput.Write(HeaderInfo, 0, 32);

                    xOutput.Seek(DataAreaPointer, SeekOrigin.Begin);

                    DataAreaPointer += file.Dump(xOutput);
                    Align256(ref DataAreaPointer);
                    EntryPointer += 32;
                }
                //Last Entry
                xOutput.Seek(EntryPointer, SeekOrigin.Begin);
                xOutput.Write(new byte[32], 0, 32);
            }
        }

        private static void Align256(ref int aValue)
        {
            if ((aValue & 0xFFFFFF00) != aValue)//256 byte aligned memory
                aValue = (int)(aValue & 0xFFFFFF00) + 0x100;
        }
    }
}

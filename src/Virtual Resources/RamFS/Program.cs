using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace RamFS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length < 3)
                    throw new Exception("No Argument; RamFS [input folder] -o [output file]");

                if (!Directory.Exists(args[0]))
                    throw new Exception("Input Directory not found!");

                if (!Path.IsPathRooted(args[0]))
                    args[0] = Path.Combine(Environment.CurrentDirectory, args[0]);

                if (!Path.IsPathRooted(args[2]))
                    args[2] = Path.Combine(Environment.CurrentDirectory, args[2]);

                using (var BW = new BinaryWriter(File.Create(args[2])))
                {
                    BW.BaseStream.Position = 1024;
                    var InputFiles = Directory.GetFiles(args[0]);

                    var Entries = new List<Tuple<string, int, int>>();
                    foreach (var xFile in InputFiles)
                    {
                        using (var SR = new StreamReader(File.Open(xFile, FileMode.Open)))
                        {
                            var xData = Encoding.ASCII.GetBytes(SR.ReadToEnd());
                            var Pos = BW.BaseStream.Position;
                            BW.Write(xData);
                            Entries.Add(new Tuple<string, int, int>(Path.GetFileName(xFile), (int)Pos, xData.Length));
                        }
                    }

                    BW.BaseStream.Position = 0;
                    Console.WriteLine("Ram Disk ----");
                    foreach(var entry in Entries)
                    {
                        BW.Write(entry.Item1);
                        BW.Write(entry.Item2);
                        BW.Write(entry.Item3);
                        Console.WriteLine(string.Format("Name:\"{0}\"  Size={1}    Position={2}", entry.Item1, entry.Item3, entry.Item2));
                    }                   
                }
                //Lets Generate The hash; this will be hard coded in kernel
                uint Hash1 = 0, Hash2 = 0, Hash3 = 0xDEAD, Hash4 = 0;
                using (var SR = new StreamReader(File.Open(args[2], FileMode.Open)))
                {
                    var xData = SR.ReadToEnd();
                    for (int i = 0; i < xData.Length; i++)
                    {
                        var key = xData[i];
                        if (key == 0x0)
                        {
                            Hash4++;
                            continue;
                        }
                        Hash1 = key + (Hash1 << 6) + (Hash1 << 16) - Hash1;
                        Hash2 = xData[xData.Length - 1 - i] + (Hash2 << 6) + (Hash2 << 16) - Hash2;
                        var AND = Hash3 & key;
                        var XOR = Hash3 ^ key;
                        var OR = Hash3 | key;
                        var MAXIMUM = Math.Max(AND, Math.Max(XOR, OR));
                        var MINIMUM = Math.Min(AND, Math.Min(XOR, OR));
                        MAXIMUM -= MINIMUM;
                        Hash3 += ~(MAXIMUM & (MAXIMUM - 1));
                    }
                    Hash4 = (uint)xData.Length - Hash4;
                }
                Console.WriteLine(string.Format("Computed Hash: {0} {1} {2} {3}", Hash1.ToString("X8"), Hash2.ToString("X8"), Hash3.ToString("X8"), Hash4.ToString("X8")));
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong!");
                Console.WriteLine(e.Message);
            }
        }
    }
}
;
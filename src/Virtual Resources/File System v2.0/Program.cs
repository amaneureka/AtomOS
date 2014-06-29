using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileSystem.ATA;
using System.IO;

namespace FileSystem
{
    public class Program
    {
        public static List<BlockDevice> Devices = new List<BlockDevice>();
        public static List<Partition> Partitions = new List<Partition>();
        static void Main(string[] args)
        {
            Stream xS = File.Open(@"..\..\Virtual Resources\File System v2.0\fs.vmdk", FileMode.Open);
            var xATA = new Atapio(xS); //just Init all code
            

            if (Devices.Count > 0)
            {
                foreach (var part in Devices)
                {
                    if (part is Partition)
                        Partitions.Add((Partition)part);
                }
            }

            var xFAT = new FAT.Fatfilesystem(Partitions[0]);
            Console.WriteLine(xFAT.ToString());
            byte[] a = new byte[512 * 8];
            Partitions[0].Read(8192, 8U, a);
            for (int i = 0; i < 512 * 8; i++ )
            {
                if (a[i] != 0x0)
                    Console.WriteLine(a[i].ToString("X2"));
            }
            Console.WriteLine("Devices:" + Devices.Count);
            Console.ReadLine();
        }
    }
}

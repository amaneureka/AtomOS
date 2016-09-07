using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernel_alpha;
using Kernel_alpha.Lib;
using Kernel_alpha.Lib.Encoding;

namespace Kernel_alpha
{
    public unsafe class Elf
    {
        elfHeaders el = new elfHeaders();

        public byte[] ParseELF(byte[] xData)
        {
            el.ident = new char[16];
            byte[] xReadData = null;
            if (ReadHeader(xData, el) == 0)
            {
                Console.WriteLine("ELF file found");
                xReadData = ParseELFData(xData, el);
            }

            return xReadData;
        }

        private byte[] ParseELFData(byte[] xData, elfHeaders el)
        {
            // starting with core program
            programHeader ph = new programHeader();
            Exe_Format exeFormat = new Exe_Format();
            exeFormat.entryAddr = el.entry;
            Console.WriteLine("Entry address : " + exeFormat.entryAddr.ToString());
            exeFormat.numSegments = el.phnum;
            exeFormat.segmentList = new Exe_Segment[el.phnum];
            byte[] xReadData = null;

            int temp = 0;
            for (int i = 0; i < el.phnum; i++)
            {
                ph.type = BitConverter.ToUInt16(xData, 0x34 + (0x20 * i));
                exeFormat.segmentList[i].offsetInFile = ph.offset = BitConverter.ToUInt16(xData, 0x34 + (temp = +4) + (0x20 * i));
                exeFormat.segmentList[i].startAddress = ph.vaddr = BitConverter.ToUInt32(xData, 0x34 + (temp = +4) + (0x20 * i));
                ph.paddr = BitConverter.ToUInt32(xData, 0x34 + (temp = +4) + (0x20 * i));
                exeFormat.segmentList[i].lengthInFile = ph.fileSize = BitConverter.ToUInt16(xData, 0x34 + (temp = +4) + (0x20 * i));
                exeFormat.segmentList[i].sizeInMemory = ph.memSize = BitConverter.ToUInt16(xData, 0x34 + (temp = +4) + (0x20 * i));
                exeFormat.segmentList[i].protFlags = ph.flags = BitConverter.ToUInt16(xData, 0x34 + (temp = +4) + (0x20 * i));
                ph.alignment = BitConverter.ToUInt16(xData, BitConverter.ToUInt16(xData, 0x34 + (temp = +4) + (0x20 * i)));

                if (ph.type == 1)
                {
                    //xReadData = new byte[exeFormat.segmentList[i].lengthInFile];

                    //for (int j = (int)exeFormat.entryAddr; j < exeFormat.entryAddr + exeFormat.segmentList[i].lengthInFile; j++)
                    //{
                    //    xReadData[j - (int)exeFormat.entryAddr] = xData[j];
                    //}
                    //break;

                }
            }

            // Reading section header
            sectionHeader sh = new sectionHeader();
            temp = 0;
            for (int i = 0; i < el.shnum; i++)
            {
                sh.sh_name = BitConverter.ToUInt16(xData, (int)el.sphoff + (el.shentsize * i));
                sh.sh_type = BitConverter.ToUInt16(xData, (int)el.sphoff + 4 + (el.shentsize * i));
                sh.sh_flags = BitConverter.ToUInt16(xData, (int)el.sphoff + 8 + (el.shentsize * i));
                sh.sh_Addr = BitConverter.ToUInt32(xData, (int)el.sphoff + 12 + (el.shentsize * i));
                sh.sh_offset = BitConverter.ToUInt32(xData, (int)el.sphoff + 16 + (el.shentsize * i));
                sh.sh_size = BitConverter.ToUInt16(xData, (int)el.sphoff + 20 + (el.shentsize * i));
                sh.sh_link = BitConverter.ToUInt16(xData, (int)el.sphoff + 24 + (el.shentsize * i));
                sh.sh_info = BitConverter.ToUInt16(xData, (int)el.sphoff + 28 + (el.shentsize * i));
                sh.sh_addralign = BitConverter.ToUInt16(xData, (int)el.sphoff + 32 + (el.shentsize * i));
                sh.sh_entsize = BitConverter.ToUInt16(xData, (int)el.sphoff + 36 + (el.shentsize * i));
            }

            //xReadData = new byte[0x180];
            //for (int j = 0x2F0; j < 0x470; j++)
            //{
            //    xReadData[j - 0x2F0] = xData[j];
            //}
            return xReadData;

        }

        public UInt16 StartOfStringTable(byte[] xData)
        {
              return  BitConverter.ToUInt16(xData, (int)el.sphoff + 16 + (el.shentsize * el.shstrndx));
        }


        public int ReadHeader(byte[] xData, elfHeaders el)
        {
            // Reading Magic Number
            for (int i = 0; i < 4; i++)
            {
                el.ident[i] = (char)xData[i];
            }
            // Reading architecture format
            el.ident[4] = (char)xData[4];
            // Endian identifier
            el.ident[5] = (char)xData[5];
            // original version for ELF
            el.ident[6] = (char)xData[6];
            el.ident[10] = (char)xData[16];
            // checking whether the elf is executable or relocatable or shared
            el.type = BitConverter.ToUInt16(xData, 16);
            // instruction set architecture
            el.machine = BitConverter.ToUInt16(xData, 18);
            // version
            el.version = BitConverter.ToUInt32(xData, 20);
            // memory address entry  point
            el.entry = BitConverter.ToUInt32(xData, 24);
            // changes from here...
            el.phoff = BitConverter.ToUInt32(xData, (int)elfEnum.phoff);
            el.sphoff = BitConverter.ToUInt32(xData, (int)elfEnum.sphoff);
            el.flag = BitConverter.ToUInt32(xData, (int)elfEnum.flag);
            el.ehsize = BitConverter.ToUInt16(xData, (int)elfEnum.ehsize);
            el.phentsize = BitConverter.ToUInt16(xData, (int)elfEnum.phentsize);
            el.phnum = BitConverter.ToUInt16(xData, (int)elfEnum.phnum);
            el.shentsize = BitConverter.ToUInt16(xData, (int)elfEnum.shentsize);
            el.shnum = BitConverter.ToUInt16(xData, (int)elfEnum.shnum);
            el.shstrndx = BitConverter.ToUInt16(xData, (int)elfEnum.shstrndx);

            if (el.ident[0] != '\x7F' || el.ident[1] != 'E' || el.ident[2] != 'L' || el.ident[3] != 'F')
                return -1;

            if (el.ident[4] == 2)
                return -1; // not targetting x64 at this time :)

            return 0;
        }
    }


    public class elfHeaders
    {
        public char[] ident { get; set; }
        public ushort type { get; set; }
        public ushort machine { get; set; }
        public uint version { get; set; }
        public UInt32 entry { get; set; }
        public UInt32 phoff { get; set; }
        public UInt32 sphoff { get; set; }
        public uint flag { get; set; }
        public ushort ehsize { get; set; }
        public ushort phentsize { get; set; }
        public ushort phnum { get; set; }
        public ushort shentsize { get; set; }
        public ushort shnum { get; set; }
        public ushort shstrndx { get; set; }
    }

    public class programHeader
    {
        public uint type { get; set; }
        public uint offset { get; set; }
        public UInt32 vaddr { get; set; }
        public UInt32 paddr { get; set; }
        public uint fileSize { get; set; }
        public uint memSize { get; set; }
        public uint flags { get; set; }
        public uint alignment { get; set; }
    }

    public class sectionHeader
    {
        public uint sh_name { get; set; }
        public uint sh_type { get; set; }
        public uint sh_flags { get; set; }
        public UInt32 sh_Addr { get; set; }
        public UInt32 sh_offset { get; set; }
        public uint sh_size { get; set; }
        public uint sh_link { get; set; }
        public uint sh_info { get; set; }
        public uint sh_addralign { get; set; }
        public uint sh_entsize { get; set; }
    }

    public class Exe_Segment
    {
       public uint offsetInFile { get; set; }	 /* Offset of segment in executable file */
       public uint lengthInFile { get; set; }	 /* Length of segment data in executable file */
       public UInt32 startAddress { get; set; }	 /* Start address of segment in user memory */
       public uint sizeInMemory { get; set; }	 /* Size of segment in memory */
       public uint protFlags { get; set; }		 /* VM protection flags; combination of VM_READ,VM_WRITE,VM_EXEC */
    }

    public class Exe_Format
    {
        public Exe_Segment[] segmentList { get; set; }  /* Definition of segments */
        public int numSegments { get; set; }		/* Number of segments contained in the executable */
        public uint entryAddr { get; set; } 	/* Code entry point address */
    }

    // targetting x86
    enum elfEnum : int
    {
        ident,
        type = 16,
        machine = 18,
        version = 20,
        entry = 24,
        phoff = 28,
        sphoff = 32,
        flag = 36,
        ehsize = 40,
        phentsize = 42,
        phnum = 44,
        shentsize = 46,
        shnum = 48,
        shstrndx = 50
    }
}

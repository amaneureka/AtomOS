/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          ELF Parsing Library
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Runtime.InteropServices;

using Atomixilc.Lib;

using Atomix.Kernel_H.IO;
using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.IO.FileSystem;

namespace Atomix.Kernel_H.Exec
{
    internal unsafe static class ELF
    {
        [StructLayout(LayoutKind.Explicit, Size = 52)]
        struct Elf_Header
        {
            [FieldOffset(0)]
            public fixed byte e_ident[16];
            [FieldOffset(16)]
            public ushort e_type;
            [FieldOffset(18)]
            public ushort e_machine;
            [FieldOffset(20)]
            public uint e_version;
            [FieldOffset(24)]
            public uint e_entry;
            [FieldOffset(28)]
            public uint e_phoff;
            [FieldOffset(32)]
            public uint e_shoff;
            [FieldOffset(36)]
            public uint e_flags;
            [FieldOffset(40)]
            public ushort e_ehsize;
            [FieldOffset(42)]
            public ushort e_phentsize;
            [FieldOffset(44)]
            public ushort e_phnum;
            [FieldOffset(46)]
            public ushort e_shentsize;
            [FieldOffset(48)]
            public ushort e_shnum;
            [FieldOffset(50)]
            public ushort e_shstrndx;
        };

        [StructLayout(LayoutKind.Explicit, Size = 40)]
        struct Elf_Shdr
        {
            [FieldOffset(0)]
            public uint sh_name;
            [FieldOffset(4)]
            public uint sh_type;
            [FieldOffset(8)]
            public uint sh_flags;
            [FieldOffset(12)]
            public uint sh_addr;
            [FieldOffset(16)]
            public uint sh_offset;
            [FieldOffset(20)]
            public uint sh_size;
            [FieldOffset(24)]
            public int sh_link;
            [FieldOffset(28)]
            public int sh_info;
            [FieldOffset(32)]
            public uint sh_addralign;
            [FieldOffset(36)]
            public uint sh_entsize;
        };

        [StructLayout(LayoutKind.Explicit, Size = 8)]
        struct Elf32_Rel
        {
            [FieldOffset(0)]
            public uint r_offset;
            [FieldOffset(4)]
            public uint r_info;
        };

        [StructLayout(LayoutKind.Explicit, Size = 16)]
        struct Elf32_Sym
        {
            [FieldOffset(0)]
            public uint st_name;
            [FieldOffset(4)]
            public uint st_value;
            [FieldOffset(8)]
            public uint st_size;
            [FieldOffset(12)]
            public byte st_info;
            [FieldOffset(13)]
            public byte st_other;
            [FieldOffset(14)]
            public ushort st_shndx;
        };

        /* Identification indexes for e_ident field in ELF header */
        const int EI_MAG0       = 0;         /* file ID byte 0 */
        const int EI_MAG1       = 1;         /* file ID byte 1 */
        const int EI_MAG2       = 2;         /* file ID byte 2 */
        const int EI_MAG3       = 3;         /* file ID byte 3 */
        const int EI_CLASS      = 4;         /* file class (capacity) */
        const int EI_DATA       = 5;         /* data encoding */
        const int EI_VERSION    = 6;         /* file version */
        const int EI_PAD        = 7;         /* start of padding bytes */
        const int EI_NIDENT     = 16;        /* size of e_ident[] */

        /* "Magic number" in e_ident field of ELF header */
        const byte ELFMAG0 = 0x7F;
        const byte ELFMAG1 = (byte)'E';
        const byte ELFMAG2 = (byte)'L';
        const byte ELFMAG3 = (byte)'F';

        /* Relocation Type for i386 machine */
        const byte R_386_NONE       = 0;
        const byte R_386_32         = 1;    /* S + A */
        const byte R_386_PC32       = 2;    /* S + A - P */
        const byte R_386_GOT32      = 3;    /* G + A */
        const byte R_386_PLT32      = 4;    /* L + A - P */
        const byte R_386_COPY       = 5;
        const byte R_386_GLOB_DAT   = 6;    /* S */
        const byte R_386_JMP_SLOT   = 7;    /* S */
        const byte R_386_RELATIVE   = 8;    /* B + A */
        const byte R_386_GOTOFF     = 9;    /* S + A - GOT */
        const byte R_386_GOTPC      = 10;   /* GOT + A - P */
        const byte R_386_32PLT      = 11;	/* L + A */

        /* File class (or capacity) in e_ident[4] of ELF header */
        const byte ELFCLASSNONE = 0;        /* invalid class */
        const byte ELFCLASS32   = 1;        /* 32-bit processor */
        const byte ELFCLASS64   = 2;        /* 64-bit processor */

        /* Data encoding in e_ident[5] of ELF header */
        const byte ELFDATANONE = 0;        /* invalid data encoding */
        const byte ELFDATA2LSB = 1;        /* little-endian format */
        const byte ELFDATA2MSB = 2;        /* big-endian format */

        /* Object file type in e_type field of ELF header */
        const ushort ET_NONE    = 0;         /* no file type */
        const ushort ET_REL     = 1;         /* relocatble file */
        const ushort ET_EXEC    = 2;         /* executable file */
        const ushort ET_DYN     = 3;         /* shared object file */
        const ushort ET_CORE    = 4;         /* core file */
        const ushort ET_LOPROC  = 0xff00;    /* processor-specific */
        const ushort ET_HIPROC  = 0xffff;    /* processor-specific */

        /* Required architecture in e_machine field of ELF header */
        const ushort EM_NONE    = 0;         /* no machine */
        const ushort EM_M32     = 1;         /* AT&T WE 32100 */
        const ushort EM_SPARC   = 2;         /* SPARC */
        const ushort EM_386     = 3;         /* Intel 80386 */
        const ushort EM_68K     = 4;         /* Motorola 68000 */
        const ushort EM_88K     = 5;         /* Motorola 88000 */
        const ushort EM_860     = 7;         /* Intel 80860 */
        const ushort EM_MIPS    = 8;         /* MIPS RS3000 */
        const ushort EM_ARM     = 40;        /* Advanced RISC Machines ARM */

        /* Object file version in e_version field of ELF header */
        const uint EV_NONE      = 0;         /* invalid version */
        const uint EV_CURRENT   = 1;         /* current version */

        /* Section's semantics in sh_type field of ELF section header */
        const uint SHT_NULL     = 0;            /* no section associated with header */
        const uint SHT_PROGBITS = 1;            /* program-defined data */
        const uint SHT_SYMTAB   = 2;            /* link editing & dynamic linking symbols */
        const uint SHT_STRTAB   = 3;            /* string table */
        const uint SHT_RELA     = 4;            /* minimal set of dynamic linking symbols */
        const uint SHT_HASH     = 5;            /* relocation entries with explicit addends */
        const uint SHT_DYNAMIC  = 6;            /* symbol hash table (dynamic linking) */
        const uint SHT_NOTE     = 7;            /* dynamic linking info */
        const uint SHT_NOBITS   = 8;            /* file-marking information */
        const uint SHT_REL      = 9;            /* relocation entries without explicit addends */
        const uint SHT_SHLIB    = 10;           /* reserved */
        const uint SHT_DYNSYM   = 11;           /* dynamic linking symbol table */
        const uint SHT_LOPROC   = 0x70000000;   /* LB for processor-specific dynamics */
        const uint SHT_HIPROC   = 0x7fffffff;   /* UB for processor-specific dynamics */
        const uint SHT_LOUSER   = 0x80000000;   /* LB for application-specific dynamics */
        const uint SHT_HIUSER   = 0x8fffffff;   /* UB for application-specific dynamics */

        /* Section's attribute flags in sh_flags field of ELF section header */
        const uint SHF_WRITE        = 0x1;         /* data writable during execution */
        const uint SHF_ALLOC        = 0x2;         /* occupies memory during execution */
        const uint SHF_EXECINSTR    = 0x4;         /* executable machine instructions */
        const uint SHF_MASKPROC     = 0xf0000000;  /* mask for processor-specific semantics */

        /* Special section indexes (reserved values) */
        const ushort SHN_UNDEF      = 0;            /* undefined section index */
        const ushort SHN_LORESERVE  = 0xff00;       /* LB (lower bound) of ELF reserved indexes */
        const ushort SHN_LOPROC     = 0xff00;       /* LB of processor-specific semantics */
        const ushort SHN_HIPROC     = 0xff1f;       /* UB of processor-specific semantics */
        const ushort SHN_ABS        = 0xfff1;       /* absolute (not relocatable) section */
        const ushort SHN_COMMON     = 0xfff2;       /* C external variables section */
        const ushort SHN_HIRESERVE  = 0xffff;       /* UB (upper bound) of ELF reserved indexes */

        /* Symbol binding */
        const uint STB_LOCAL        = 0;            /* Local scope */
        const uint STB_GLOBAL       = 1;            /* Global scope */
        const uint STB_WEAK         = 2;            /* Weak, (ie. __attribute__((weak))) */

        internal static uint Load(string aPath)
        {
            Stream xStream = VirtualFileSystem.GetFile(aPath);

            if (xStream == null)
                throw new Exception("[ELF]: File not found!");

            var xData = new byte[xStream.FileSize];
            xStream.Read(xData, xData.Length);

            uint BaseAddress = xData.GetDataOffset();
            Elf_Header* Header = (Elf_Header*)BaseAddress;

            /* verify ELF header and if this code support this type of elf */
            CheckHeader(Header);

            /* prepare sections and allocate memory (if required) */
            Elf_Shdr* Shdr = (Elf_Shdr*)(BaseAddress + Header->e_shoff);
            for (int i = 0; i < Header->e_shnum; i++, Shdr++)
            {
                Shdr->sh_addr = BaseAddress + Shdr->sh_offset;
                if ((Shdr->sh_flags & SHF_ALLOC) != 0)
                {
                    LoadSection(BaseAddress, Shdr);
                }
            }

            /* Iterate over all sections and perform relocations */
            Shdr = (Elf_Shdr*)(BaseAddress + Header->e_shoff);
            for (int i = 0; i < Header->e_shnum; i++, Shdr++)
            {
                switch (Shdr->sh_type)
                {
                    case SHT_SYMTAB:
                        {
                            RegisterSymbol(Header, Shdr, aPath);
                        }
                        break;
                    case SHT_REL:
                        {
                            Shdr->sh_addr = BaseAddress + Shdr->sh_offset;
                            Relocate(Header, Shdr);
                        }
                        break;
                }
            }

            uint LoadAddress = Header->e_entry;
            Heap.Free(xData);

            return LoadAddress;
        }

        private static void Relocate(Elf_Header* aHeader, Elf_Shdr* aShdr)
        {
            uint BaseAddress = (uint)aHeader;
            Elf32_Rel* Reloc = (Elf32_Rel*)aShdr->sh_addr;
            Elf_Shdr* TargetSection = (Elf_Shdr*)(BaseAddress + aHeader->e_shoff) + aShdr->sh_info;

            uint RelocCount = aShdr->sh_size / aShdr->sh_entsize;

            byte SymIdx;
            uint SymVal, RelocType;
            for (uint i = 0; i < RelocCount; i++, Reloc++)
            {
                SymVal = 0;
                SymIdx = (byte)(Reloc->r_info >> 8);
                RelocType = Reloc->r_info & 0xFF;

                if (SymIdx != SHN_UNDEF)
                {
                    if (RelocType == R_386_PLT32)
                        SymVal = 0;
                    else
                        SymVal = GetSymValue(aHeader, TargetSection->sh_link, SymIdx);
                }

                uint* add_ref = (uint*)(TargetSection->sh_addr + Reloc->r_offset);
                switch (RelocType)
                {
                    case R_386_32:
                        *add_ref = SymVal + *add_ref; // S + A
                        break;
                    case R_386_PLT32:   // L + A - P
                    case R_386_PC32:    // S + A - P
                    default:
                        throw new Exception("[ELF]: Unsupported Relocation type");
                }
            }
        }

        private static void RegisterSymbol(Elf_Header* aHeader, Elf_Shdr* aShdr, string aPath)
        {
            uint BaseAddress = (uint)aHeader;
            Elf32_Sym* SymTab = (Elf32_Sym*)aShdr->sh_addr;
            var StrTabAdd = ((Elf_Shdr*)(BaseAddress + aHeader->e_shoff) + aShdr->sh_link)->sh_addr;

            uint count = aShdr->sh_size / aShdr->sh_entsize;

            uint Address;
            for (uint i = 0; i < count; i++, SymTab++)
            {
                uint flag = (uint)(SymTab->st_info >> 4);
                if (flag == STB_GLOBAL)
                {
                    switch (SymTab->st_shndx)
                    {
                        case SHN_UNDEF:
                            continue; // for now ignore UNDEF Symbols
                        case SHN_ABS:
                            Address = SymTab->st_value;
                            break;
                        default:
                            var TargetSection = (Elf_Shdr*)(BaseAddress + aHeader->e_shoff) + SymTab->st_shndx;
                            Address = TargetSection->sh_addr + SymTab->st_value;
                            break;
                    }

                    string SymName = new string((sbyte*)(StrTabAdd + SymTab->st_name));
                    Debug.Write("Symbol: %s\n", SymName);
                }
            }
        }

        private static uint GetSymValue(Elf_Header* aHeader, int aTableIdx, int aSymIdx)
        {
            uint BaseAddress = (uint)aHeader;
            Elf_Shdr* SymSection = (Elf_Shdr*)(BaseAddress + aHeader->e_shoff) + aTableIdx;
            Elf32_Sym* SymTab = (Elf32_Sym*)(SymSection->sh_addr) + aSymIdx;

            switch (SymTab->st_shndx)
            {
                case SHN_UNDEF:
                    {
                        var StrTabAdd = ((Elf_Shdr*)(BaseAddress + aHeader->e_shoff) + SymSection->sh_link)->sh_addr;
                        string SymName = new string((sbyte*)(StrTabAdd + SymTab->st_name));

                        Debug.Write("Undefined Symbol: %s\n", SymName);
                        Heap.Free(SymName);
                        throw new Exception("[ELF]: Extern Symbol not supported");
                    }
                case SHN_ABS:
                    return SymTab->st_value;
                default:
                    Elf_Shdr* TargetSection = (Elf_Shdr*)(BaseAddress + aHeader->e_shoff) + SymTab->st_shndx;
                    return TargetSection->sh_addr + SymTab->st_value;
            }
        }

        private static uint LoadSection(uint aBaseAddress, Elf_Shdr* aShdr)
        {
            // make sure address if aligned
            uint align = aShdr->sh_addralign;
            uint addr = Heap.kmalloc(aShdr->sh_size + align);

            if (align != 0)
            {
                uint offset = addr & (align - 1);
                addr += align - offset;
            }

            if (aShdr->sh_type != SHT_NOBITS)
                Memory.FastCopy(addr, aBaseAddress + aShdr->sh_offset, aShdr->sh_size);

            aShdr->sh_addr = addr;
            return addr;
        }

        private static void CheckHeader(Elf_Header* aHeader)
        {
            /* Check if we are supporting this standard */
            var ident = aHeader->e_ident;
            if ((ident[EI_MAG0] != ELFMAG0) ||
                (ident[EI_MAG1] != ELFMAG1) ||
                (ident[EI_MAG2] != ELFMAG2) ||
                (ident[EI_MAG3] != ELFMAG3))
                throw new Exception("[ELF]: Invalid File format");

            if (ident[EI_CLASS] != ELFCLASS32)
                throw new Exception("[ELF]: Unsupported EI_CLASS");

            if (ident[EI_DATA] != ELFDATA2LSB)
                throw new Exception("[ELF]: Unsupported EI_DATA");

            if (aHeader->e_machine != EM_386)
                throw new Exception("[ELF]: Unsupported Machine Type");

            if (aHeader->e_type != ET_REL)
                throw new Exception("[ELF]: Unsupported ELF Type");

            if (aHeader->e_version != EV_CURRENT)
                throw new Exception("[ELF]: Unsupported ELF Version");
        }
    }
}

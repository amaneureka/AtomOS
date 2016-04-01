using System;
using System.Runtime.InteropServices;

using Atomix.Kernel_H.io;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.exec
{
    public unsafe static class ELF
    {
        #region Enum
        enum Elf_Ident : int
        {
            EI_MAG0         = 0, // 0x7F
            EI_MAG1         = 1, // 'E'
            EI_MAG2         = 2, // 'L'
            EI_MAG3         = 3, // 'F'
            EI_CLASS        = 4, // Architecture (32/64)
            EI_DATA         = 5, // Byte Order
            EI_VERSION      = 6, // ELF Version
            EI_OSABI        = 7, // OS Specific
            EI_ABIVERSION   = 8, // OS Specific
            EI_PAD          = 9  // Padding
        };
        enum Elf_Type : int
        {
            ET_NONE         = 0, // Unkown Type
            ET_REL          = 1, // Relocatable File
            ET_EXEC         = 2  // Executable File
        };
        enum ShT_Types : int
        {
            SHT_NULL        = 0,   // Null section
            SHT_PROGBITS    = 1,   // Program information
            SHT_SYMTAB      = 2,   // Symbol table
            SHT_STRTAB      = 3,   // String table
            SHT_RELA        = 4,   // Relocation (w/ addend)
            SHT_NOBITS      = 8,   // Not present in file
            SHT_REL         = 9,   // Relocation (no addend)
        };
        enum ShT_Attributes : byte
        {
            SHF_WRITE       = 0x01, // Writable section
            SHF_ALLOC       = 0x02  // Exists in memory
        };
        enum RtT_Types : int
        {
            R_386_NONE      = 0,    // No relocation
            R_386_32        = 1,    // Symbol + Offset
            R_386_PC32      = 2     // Symbol + Offset - Section Offset
        };
        enum StT_Bindings : int
        {
            STB_LOCAL       = 0,    // Local scope
            STB_GLOBAL      = 1,    // Global scope
            STB_WEAK        = 2     // Weak, (ie. __attribute__((weak)))
        };

        enum StT_Types : int
        {
            STT_NOTYPE      = 0,    // No type
            STT_OBJECT      = 1,    // Variables, arrays, etc.
            STT_FUNC        = 2     // Methods or functions
        };
        #endregion
        #region Constants
        const byte ELFMAG0 = 0x7F;
        const byte ELFMAG1 = (byte)'E';
        const byte ELFMAG2 = (byte)'L';
        const byte ELFMAG3 = (byte)'F';
        const byte ELFCLASS32 = 1;  // Little Endian
        const byte ELFDATA2LSB = 1; //32-bit Architecture
        const byte EM_386 = 3; // x86 Machine Type
        const ushort SHN_ABS = 0xFFF1;
        const ushort SHN_UNDEF = 0x0;
        #endregion
        #region Structure
        [StructLayout(LayoutKind.Explicit, Size=40)]
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
            public uint sh_link;
            [FieldOffset(28)]
            public uint sh_info;
            [FieldOffset(32)]
            public uint sh_addralign;
            [FieldOffset(36)]
            public uint sh_entsize;
        };
        [StructLayout(LayoutKind.Explicit, Size=52)]
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
        #endregion

        public static void Load(Stream aStream)
        {
            if (aStream == null)
                throw new Exception("[ELF]: Stream closed");

            var xData = new byte[aStream.FileSize];
            aStream.Read(xData, xData.Length);

            //because 0x10 bytes are dedicated for array meta information
            var header = (Elf_Header*)(Native.GetAddress(xData) + 0x10);
            
            try
            {
                /* Check if we are supporting this standard */
                var ident = header->e_ident;
                if (ident[(int)Elf_Ident.EI_MAG0] != ELFMAG0
                    || ident[(int)Elf_Ident.EI_MAG1] != ELFMAG1
                    || ident[(int)Elf_Ident.EI_MAG2] != ELFMAG2
                    || ident[(int)Elf_Ident.EI_MAG3] != ELFMAG3)
                    throw new Exception("[ELF]: Invalid File format");

                if (ident[(int)Elf_Ident.EI_CLASS] != ELFCLASS32)
                    throw new Exception("[ELF]: Unsupported EI_CLASS");

                if (ident[(int)Elf_Ident.EI_DATA] != ELFDATA2LSB)
                    throw new Exception("[ELF]: Unsupported EI_DATA");

                if (header->e_machine != EM_386)
                    throw new Exception("[ELF]: Unsupported Machine Type");

                if ((Elf_Type)header->e_type != Elf_Type.ET_REL)
                    throw new Exception("[ELF]: Unsupported ELF Type");
                
                /* Iterate over all sections present in file */
                var section = elf_section(header, 0);
                for (int index = 0; index < header->e_shnum; index++, section++)
                {
                    Debug.Write("Type: %d ", section->sh_type);
                    Debug.Write("Size: %d\n", section->sh_size);
                    switch ((ShT_Types)section->sh_type)
                    {
                        case ShT_Types.SHT_NOBITS:
                            {
                                //Ignore if size == 0
                                if (section->sh_size == 0)
                                    continue;

                                //do we really have to copy it to memory?
                                if ((section->sh_flags & (int)ShT_Attributes.SHF_ALLOC) != 0)
                                {
                                    var Mem = Heap.kmalloc(section->sh_size);
                                    section->sh_offset = Mem - (uint)header;
                                }
                            }
                            break;
                        case ShT_Types.SHT_REL:
                            {
                                Elf32_Rel* relTable = (Elf32_Rel*)((uint)header + section->sh_offset);
                                Elf_Shdr* targetSection = elf_section(header, section->sh_info);//move to elf section header
                                uint sectionOffset = (uint)header + targetSection->sh_offset;
                                for (int i = 0; i < section->sh_size / section->sh_entsize; i++, relTable++)
                                {
                                    uint SymValue = 0;
                                    uint SymIndex = (relTable->r_info >> 8);
                                    if (SymIndex != 0)
                                    {
                                        //Lookup symbol
                                        var SymbolSection = elf_section(header, targetSection->sh_link);

                                        var SymbolTable = (Elf32_Sym*)(SymbolSection->sh_offset + (uint)header);
                                        SymbolTable += SymIndex;

                                        if (SymbolTable->st_shndx == SHN_UNDEF)
                                            throw new Exception("[ELF]: Extern Symbol unsupported");
                                        else if (SymbolTable->st_shndx == SHN_ABS)
                                            SymValue = SymbolTable->st_value;
                                        else
                                        {
                                            var target = elf_section(header, SymbolTable->st_shndx);
                                            SymValue = (uint)header + SymbolTable->st_value + target->sh_offset;
                                        }
                                    }

                                    uint* addr_ref = (uint*)(sectionOffset + relTable->r_offset);

                                    //Perform Relocation
                                    switch((RtT_Types)(relTable->r_info & 0xFF))
                                    {
                                        case RtT_Types.R_386_32:
                                            *addr_ref = *addr_ref + SymValue;
                                            break;
                                        case RtT_Types.R_386_PC32:
                                            *addr_ref = *addr_ref + SymValue - (uint)addr_ref;
                                            break;
                                        default:
                                            throw new Exception("[ELF]: Relocation type unsupported");
                                    }
                                }
                            }
                            break;
                        case ShT_Types.SHT_SYMTAB:
                            {
                                var SymbolTable = (Elf32_Sym*)(section->sh_offset + (uint)header);

                                uint count = section->sh_size / section->sh_entsize;
                                for (uint idx = 0; idx < count; idx++, SymbolTable++)
                                {
                                    Debug.Write("Binding: %d ", (uint)(SymbolTable->st_info >> 4));
                                    Debug.Write("Type: %d ", (uint)(SymbolTable->st_info & 0x0F));
                                    Debug.Write("shndx: %d ", SymbolTable->st_shndx);
                                    Debug.Write("st_value: %d ", SymbolTable->st_value);
                                    Debug.Write("name: %d\n", SymbolTable->st_name);
                                    if ((StT_Bindings)(SymbolTable->st_info >> 4) == StT_Bindings.STB_GLOBAL)
                                    {
                                        var target = elf_section(header, SymbolTable->st_shndx);
                                        var Address = (uint)header + target->sh_offset + SymbolTable->st_value;
                                        Debug.Write("Global Address: %d\n", Address);
                                    }
                                }
                            }
                            break;
                    }

                    //Copy segment to memory
                    if ((ShT_Types)section->sh_type != ShT_Types.SHT_NOBITS)
                    {
                        //Copy section to memory
                        var des = Heap.kmalloc(section->sh_size);
                        var src = (uint)header + section->sh_offset;
                        Memory.FastCopy(des, src, section->sh_size);
                        section->sh_offset = des - (uint)header;
                    }
                }
            }
            catch (Exception e)
            {
                Heap.Free(xData);
                throw e;
            }
        }

        private static Elf_Shdr* elf_section(Elf_Header *header, uint aOffset)
        {
            var table = (Elf_Shdr*)(header->e_shoff + (uint)header);
            return table + aOffset;
        }
    }
}

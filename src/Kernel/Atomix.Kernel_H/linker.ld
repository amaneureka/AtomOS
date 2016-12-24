ENTRY(entrypoint)
OUTPUT_FORMAT(elf32-i386)

SECTIONS
{
   . = 0xC0100000;

   .text : AT(ADDR(.text) - 0xC0000000)
   {
       *(.text)
       *(.rodata*)
   }

   .data ALIGN (0x1000) : AT(ADDR(.data) - 0xC0000000)
   {
       *(.data)
	   *(.symbols)
   }

   .bss : AT(ADDR(.bss) - 0xC0000000)
   {
       *(COMMON)
       *(.bss)
   }

   .eh_frame : AT(ADDR(.eh_frame) - 0xC0000000)
   {
       *(.eh_frame)
   }

   	/DISCARD/ :
	{
		*(.comment)
		*(.note.gnu.build-id)
	}
}
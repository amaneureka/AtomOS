ENTRY(Kernel_Main)
OUTPUT_FORMAT(elf32-i386)

SECTIONS
{
   . = 0x100000;

   .text : AT(ADDR(.text) - 0x00000000)
   {
       *(.text)
       *(.rodata*)
   }

   .data ALIGN (0x1000) : AT(ADDR(.data) - 0x00000000)
   {
       *(.data)
   }

   .bss : AT(ADDR(.bss) - 0x00000000)
   {
       _sbss = .;
       *(COMMON)
       *(.bss)
       _ebss = .;
   }
}
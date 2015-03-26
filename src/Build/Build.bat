@echo off
Title Atomix Batch Builder
REM MAKE IT SOMEHOW MANAGED AND BEAUTIFUL
chdir "Bin\"
::Atomixilc.exe -cpu x86 -i Kernel_alpha.dll;Atomix.mscorlib.dll -o "D:\Aman Priyadarshi Private\Atom OS IL2ASM\Build\Debug" -d
Atomixilc.exe -cpu x86 -i Atomix.Kernel_H.dll -o "D:\Aman Priyadarshi Private\Atom OS IL2ASM\Build\Debug" -d
REM I'm not going to add NAsm too, because a dev. is working on 32 bit assembler in c#
REM Also Not going to add iso maker here, because i want to have it managed in c#
cd "D:\Aman Priyadarshi Private\Atom OS IL2ASM\Build\Debug"
call Builder2.bat
cd "D:\Atomix\src\Build"
echo "Compilation process done"
echo %time%

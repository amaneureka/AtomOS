@echo off
Title Atomix Batch Builder
::msbuild /p:Configuration=Release "..\Atomix.sln"
".\Bin\Atomixilc.exe" -cpu x86 -i ".\Bin\Atomix.Kernel_H.dll" -o ".\Output" -d -optimize
::".\Bin\Atomixilc.exe" -cpu x86 -i ".\Bin\Kernel_alpha.dll;.\Bin\Atomix.mscorlib.dll" -o ".\Output" -d
::".\Bin\RamFS.exe" ".\ramdisk" -o ".\ISO\Initrd.bin"
chdir ".\Output"
nasm.exe -fbin ".\Kernel.asm" -o "..\ISO\Kernel.bin"
"..\mkisofs.exe" -o AtomOS-Alfa.iso -b isolinux/isolinux.bin -c isolinux/boot.cat -no-emul-boot -boot-load-size 4 -boot-info-table "../ISO"
echo Build Successfully
echo %time%
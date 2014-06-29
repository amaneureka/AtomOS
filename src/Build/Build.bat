@echo off
Title Atomix Batch Builder
REM MAKE IT SOMEHOW MANAGED AND LOOK BEAUTIFUL
"Bin/Atomixilc.exe" -cpu x86 -i Bin/Kernel_alpha.dll;Bin/Atomix.mscorlib.dll -o "Output/" -d
REM I'm not going to add NAsm too, because a dev. is working on 32 bit assembler in c#
REM Also Not going to add iso maker here, because i want to have it managed in c#
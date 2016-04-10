@echo off

REM  █████╗ ████████╗ ██████╗ ███╗   ███╗██╗██╗  ██╗     ██████╗ ███████╗
REM ██╔══██╗╚══██╔══╝██╔═══██╗████╗ ████║██║╚██╗██╔╝    ██╔═══██╗██╔════╝
REM ███████║   ██║   ██║   ██║██╔████╔██║██║ ╚███╔╝     ██║   ██║███████╗
REM ██╔══██║   ██║   ██║   ██║██║╚██╔╝██║██║ ██╔██╗     ██║   ██║╚════██║
REM ██║  ██║   ██║   ╚██████╔╝██║ ╚═╝ ██║██║██╔╝ ██╗    ╚██████╔╝███████║
REM ╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝     ╚═╝╚═╝╚═╝  ╚═╝     ╚═════╝ ╚══════╝

REM CONFIGURATION
SET BUILD_CPU=x86
SET BUILD_NAME=AtomOS-Alfa
SET ATOMIX_APPS=.\Bin\Apps
SET OUTPUT_DIR=.\Output
SET ATOMIX_COMPILER_FLAGS=-d -optimize
SET ATOMIX_COMPILER=.\Bin\Atomixilc.exe
SET ATOMIX_KERNEL=.\Bin\Atomix.Kernel_H.dll
SET ATOMIX_ISO_DIR=.\ISO
SET ATOMIX_RD=.\ramdisk
SET ATOMIX_RamFS=.\Bin\Atomix.RamFS.exe

REM CONSTANTS
SET /A EXIT_CODE=0
SET /A ERROR_COMPILER_MISSING=1
SET /A ERROR_KERNEL_MISSING=2

REM BASIC CHECKING
IF NOT EXIST %ATOMIX_COMPILER% SET /A EXIT_CODE^|=%ERROR_COMPILER_MISSING%
IF NOT EXIST %ATOMIX_KERNEL% SET /A EXIT_CODE^|=%ERROR_KERNEL_MISSING%
IF /I "%EXIT_CODE%" NEQ "0" GOTO BUILDER_EXIT

REM BUILD KERNEL FIRST
%ATOMIX_COMPILER% -cpu %BUILD_CPU% -i %ATOMIX_KERNEL% -o %OUTPUT_DIR%\Kernel.asm %ATOMIX_COMPILER_FLAGS%
IF NOT EXIST %OUTPUT_DIR%\Kernel.asm GOTO BUILDER_EXIT
nasm.exe -fbin %OUTPUT_DIR%\Kernel.asm -o %ATOMIX_ISO_DIR%\Kernel.bin

REM BUILD APP ONE BY ONE
FOR %%I IN (%ATOMIX_APPS%\*.dll) DO (
	ECHO [APP] %%~nI
	%ATOMIX_COMPILER% -cpu %BUILD_CPU% -i %%I -o %OUTPUT_DIR%\App\%%~nI.asm %ATOMIX_COMPILER_FLAGS%
	IF NOT EXIST %OUTPUT_DIR%\App\%%~nI.asm GOTO BUILDER_EXIT
	nasm.exe -felf %OUTPUT_DIR%\App\%%~nI.asm -o %ATOMIX_RD%\Apps\%%~nI.bin
)

REM CREATE RAM DISK
%ATOMIX_RamFS% %ATOMIX_RD% -o %ATOMIX_ISO_DIR%\Initrd.bin

REM CREATE ISO IMAGE
mkisofs.exe -o %OUTPUT_DIR%\%BUILD_NAME%.iso -b isolinux/isolinux.bin -c isolinux/boot.cat -no-emul-boot -boot-load-size 4 -boot-info-table %ATOMIX_ISO_DIR%

:BUILDER_EXIT
IF /I "%EXIT_CODE%" NEQ "0" ECHO BUILD FAILED (%EXIT_CODE%)
IF /I "%EXIT_CODE%" EQU "0" ECHO BUILD SUCCESSFULLY
EXIT /B %EXIT_CODE%
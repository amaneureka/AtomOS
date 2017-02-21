#!/bin/bash
set -ex

#  █████╗ ████████╗ ██████╗ ███╗   ███╗██╗██╗  ██╗     ██████╗ ███████╗
# ██╔══██╗╚══██╔══╝██╔═══██╗████╗ ████║██║╚██╗██╔╝    ██╔═══██╗██╔════╝
# ███████║   ██║   ██║   ██║██╔████╔██║██║ ╚███╔╝     ██║   ██║███████╗
# ██╔══██║   ██║   ██║   ██║██║╚██╔╝██║██║ ██╔██╗     ██║   ██║╚════██║
# ██║  ██║   ██║   ╚██████╔╝██║ ╚═╝ ██║██║██╔╝ ██╗    ╚██████╔╝███████║
# ╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝     ╚═╝╚═╝╚═╝  ╚═╝     ╚═════╝ ╚══════╝

BUILD_CPU=x86
BUILD_NAME=AtomOS-Alfa
ATOMIX_APPS=./Bin/Apps
OUTPUT_DIR=./Output
ATOMIX_COMPILER_FLAGS="-v -optimize"
ATOMIX_COMPILER=./Bin/Atomixilc.exe
ATOMIX_ISO_DIR=./ISO
ATOMIX_RD=./ramdisk
ATOMIX_LIB=./Bin/Atomix.Core.dll
ATOMIX_RamFS=./Bin/Atomix.RamFS.exe
GCC_LIB=./Local/Lib

ATOMIX_KERNEL_H=./Bin/Atomix.Kernel_H.dll
ATOMIX_KERNEL_H_LINKER=./../Kernel/Atomix.Kernel_H/linker.ld

if [ "$1" = "--mono" ]; then
	PREFIX=mono
fi

# BUILD KERNEL FIRST
$PREFIX $ATOMIX_COMPILER -cpu $BUILD_CPU -i $ATOMIX_KERNEL_H -o $OUTPUT_DIR/Kernel.asm $ATOMIX_COMPILER_FLAGS
nasm -felf $OUTPUT_DIR/Kernel.asm -o $ATOMIX_ISO_DIR/Kernel.o

i386-atomos-ld $ATOMIX_ISO_DIR/Kernel.o Local/i386-atomos/lib/crti.o $GCC_LIB/gcc/i386-atomos/5.3.0/crtbegin.o $GCC_LIB/libcairo.a $GCC_LIB/libpng15.a $GCC_LIB/libpixman-1.a $GCC_LIB/libz.a $GCC_LIB/libfreetype.a $GCC_LIB/gcc/i386-atomos/5.3.0/crtend.o Local/i386-atomos/lib/crtn.o Local/i386-atomos/lib/libm.a $GCC_LIB/gcc/i386-atomos/5.3.0/libgcc.a Local/i386-atomos/lib/libc.a -T $ATOMIX_KERNEL_H_LINKER -o $ATOMIX_ISO_DIR/Kernel.bin

rm $ATOMIX_ISO_DIR/Kernel.o
readelf --wide --symbols $ATOMIX_ISO_DIR/Kernel.bin > Kernel.map

# BUILD APP ONE BY ONE
#for file in $ATOMIX_APPS/*.dll; do
#    FILE_NAME="$(basename $file .dll)"
#    echo "[APP] $file"
#    $PREFIX $ATOMIX_COMPILER -cpu $BUILD_CPU -i "$ATOMIX_APPS/$FILE_NAME.dll;$ATOMIX_LIB" -o "$OUTPUT_DIR/Apps/$FILE_NAME.asm" $ATOMIX_COMPILER_FLAGS
#    nasm -felf "$OUTPUT_DIR/Apps/$FILE_NAME.asm" -o "$ATOMIX_RD/$FILE_NAME.o"
#done

#CREATE RAM DISK
$PREFIX $ATOMIX_RamFS $ATOMIX_RD -o $ATOMIX_ISO_DIR/Initrd.bin

#CREATE ISO IMAGE
genisoimage.exe -o $OUTPUT_DIR/$BUILD_NAME.iso -b isolinux/isolinux.bin -c isolinux/boot.cat -no-emul-boot -boot-load-size 4 -input-charset utf-8 -boot-info-table $ATOMIX_ISO_DIR

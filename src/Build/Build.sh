#!/bin/bash

#  █████╗ ████████╗ ██████╗ ███╗   ███╗██╗██╗  ██╗     ██████╗ ███████╗
# ██╔══██╗╚══██╔══╝██╔═══██╗████╗ ████║██║╚██╗██╔╝    ██╔═══██╗██╔════╝
# ███████║   ██║   ██║   ██║██╔████╔██║██║ ╚███╔╝     ██║   ██║███████╗
# ██╔══██║   ██║   ██║   ██║██║╚██╔╝██║██║ ██╔██╗     ██║   ██║╚════██║
# ██║  ██║   ██║   ╚██████╔╝██║ ╚═╝ ██║██║██╔╝ ██╗    ╚██████╔╝███████║
# ╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝     ╚═╝╚═╝╚═╝  ╚═╝     ╚═════╝ ╚══════╝

BUILD_CPU=x86
BUILD_NAME=AtomOS-Alfa
ATOMIX_APPS=Bin/Apps
OUTPUT_DIR=Output
ATOMIX_COMPILER_FLAGS="-d -optimize"
ATOMIX_COMPILER=Bin/Atomixilc.exe
ATOMIX_KERNEL=Bin/Atomix.Kernel_H.dll
ATOMIX_ISO_DIR=ISO
ATOMIX_RD=ramdisk
ATOMIX_LIB=Bin/Atomix.Core.dll
ATOMIX_RamFS=Bin/Atomix.RamFS.exe

#BUILD KERNEL FIRST
mono $ATOMIX_COMPILER -cpu $BUILD_CPU -i $ATOMIX_KERNEL -o $OUTPUT_DIR/Kernel.asm $ATOMIX_COMPILER_FLAGS
nasm -fbin $OUTPUT_DIR/Kernel.asm -o $ATOMIX_ISO_DIR/Kernel.bin

#BUILD APP ONE BY ONE
for file in "$ATOMIX_APPS/*.dll"
do
    FILE_NAME="$(basename $file .dll)"
    echo "[APP] $FILE_NAME"
    mono $ATOMIX_COMPILER -cpu $BUILD_CPU -i "$ATOMIX_APPS/$FILE_NAME.dll;$ATOMIX_LIB" -o "$OUTPUT_DIR/Apps/$FILE_NAME.asm" $ATOMIX_COMPILER_FLAGS
    nasm -felf "$OUTPUT_DIR/Apps/$FILE_NAME.asm" -o "$ATOMIX_RD/$FILE_NAME.o"
done

#CREATE RAM DISK
mono $ATOMIX_RamFS $ATOMIX_RD -o $ATOMIX_ISO_DIR/Initrd.bin

#CREATE ISO IMAGE
mkisofs -o $OUTPUT_DIR/AtomOS-Alfa.iso -b isolinux/isolinux.bin -c isolinux/boot.cat -no-emul-boot -boot-load-size 4 -boot-info-table $ATOMIX_ISO_DIR

#!/bin/bash

export TARGET=i386-atomos
export ROOT=$(pwd)
export PREFIX="$ROOT/../src/Build/Local"
export SOURCES="$ROOT/../src/Build/Temp"
export TARBALLS="$ROOT/../tarballs"
export PATH=/usr/bin:$PREFIX/bin

LIB_URL=http://excellmedia.dl.sourceforge.net/project/libpng/libpng15/1.5.27/libpng-1.5.27.tar.gz
LIB_FOLDER=libpng-1.5.27

bail()
{
	echo -e "\033[1;31mBuild failed. Please check the logs above to see what went wrong.\033[0m"
	exit 1
}

if [ ! -d $LIB_FOLDER ]; then
	if [ ! -f "$TARBALLS/$LIB_FOLDER.tar.gz" ]; then
		wget -O "$TARBALLS/$LIB_FOLDER.tar.gz" $LIB_URL || bail
	fi
	tar -xvzf "$TARBALLS/$LIB_FOLDER.tar.gz" -C $ROOT
fi

pushd "$ROOT/../src/Build/Bin" || bail

	if [ -d $LIB_FOLDER ]; then
		rm -rf $LIB_FOLDER
	fi

	mkdir $LIB_FOLDER || bail

	pushd $LIB_FOLDER || bail

		$ROOT/$LIB_FOLDER/configure --prefix=$PREFIX --target=$TARGET --disable-shared || bail
		make -j4 || bail
		make -j4 install || bail

	popd

popd

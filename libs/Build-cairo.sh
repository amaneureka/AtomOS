#!/bin/bash

export TARGET=i386-atomos
export ROOT=$(pwd)
export PREFIX="$ROOT/../src/Build/Local"
export SOURCES="$ROOT/../src/Build/Temp"
export TARBALLS="$ROOT/../tarballs"
export PATCHFILES="$ROOT/../toolchain"
export PATH=/usr/bin:$PREFIX/bin

LIB_URL=https://www.cairographics.org/releases/cairo-1.14.2.tar.xz
LIB_FOLDER=cairo-1.14.2

bail()
{
	echo -e "\033[1;31mBuild failed. Please check the logs above to see what went wrong.\033[0m"
	exit 1
}

if [ ! -d $LIB_FOLDER ]; then
	if [ ! -f "$TARBALLS/$LIB_FOLDER.tar.gz" ]; then
		wget -O "$TARBALLS/$LIB_FOLDER.tar.gz" $LIB_URL || bail
	fi
	tar -xvf "$TARBALLS/$LIB_FOLDER.tar.gz" -C $ROOT
	pushd $ROOT/$LIB_FOLDER || bail
		patch -p1 -i "$PATCHFILES/$LIB_FOLDER.diff" || bail
	popd
fi

pushd "$ROOT/../src/Build/Bin" || bail

	if [ -d $LIB_FOLDER ]; then
		rm -rf $LIB_FOLDER
	fi

	mkdir $LIB_FOLDER || bail

	pushd $LIB_FOLDER || bail

		CPPFLAGS="-I$PREFIX/include" LDFLAGS="-L$PREFIX/lib" PKG_CONFIG_PATH=$PREFIX/lib/pkgconfig $ROOT/$LIB_FOLDER/configure --prefix=$PREFIX --host=$TARGET --enable-ps=no --enable-pdf=no --enable-interpreter=no --enable-xlib=no || bail
		cp "$PATCHFILES/cairo-Makefile" test/Makefile || bail
		cp "$PATCHFILES/cairo-Makefile" perf/Makefile || bail
		echo -e "\n\n#define CAIRO_NO_MUTEX 1" >> config.h || bail
		make -j4 || bail
		make -j4 install || bail

	popd

popd

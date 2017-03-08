#!/usr/bin/env bash

ROOT="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

export TARGET=i386-atomos
export PREFIX="$ROOT/../src/Build/Local"
export SOURCES="$ROOT/../src/Build/Temp"
export TARBALLS="$ROOT/../tarballs"
export PATCHFILES="$ROOT/../toolchain"
export PATH=$PREFIX/bin:/usr/bin:$PATH

LIB_URL=https://www.cairographics.org/releases/pixman-0.26.2.tar.gz
LIB_FOLDER=pixman-0.26.2

bail()
{
	echo -e "\033[1;31mBuild failed. Please check the logs above to see what went wrong.\033[0m"
	exit 1
}

if [ ! -d $LIB_FOLDER ]; then
	if [ ! -f "$TARBALLS/$LIB_FOLDER.tar.gz" ]; then
		wget -O "$TARBALLS/$LIB_FOLDER.tar.gz" $LIB_URL || bail
	fi
	tar -xvzf "$TARBALLS/$LIB_FOLDER.tar.gz" -C $ROOT >> Setup.log 2>&1
	pushd $ROOT/$LIB_FOLDER || bail
		patch -p1 -f -i "$PATCHFILES/$LIB_FOLDER.diff" || bail
	popd
fi

pushd "$ROOT/../src/Build/Bin" || bail

	if [ -d $LIB_FOLDER ]; then
		rm -rf $LIB_FOLDER
	fi

	mkdir $LIB_FOLDER || bail

	pushd $LIB_FOLDER || bail

		$ROOT/$LIB_FOLDER/configure --prefix=$PREFIX --host=$TARGET --disable-shared >> Setup.log 2>&1 || bail
		make -j4 >> Setup.log || bail
		make -j4 install >> Setup.log || bail

	popd

popd

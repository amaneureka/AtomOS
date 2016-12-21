#!/bin/bash

export TARGET=i386-atomos
export PREFIX="$(pwd)/Local"
export SOURCES="$(pwd)/temp"
export TARBALLS="$(pwd)/../../tarballs"
export PATCHFILES="$(pwd)/../../toolchain"
export PATH=/usr/bin:$PREFIX/bin

BUILD_GCC=false
BUILD_GCC_2=false
BUILD_NEWLIB=false
BUILD_BINUTILS=false
BUILD_AUTOCONF=false
BUILD_AUTOMAKE=false

bail()
{
	echo -e "\033[1;31mBuild failed. Please check the logs above to see what went wrong.\033[0m"
	exit 1
}

message()
{
	echo -e "\033[1;36m$1\033[0m"
}

reply()
{
	echo -e "\033[1;37m$1\033[0m"
}

fetch()
{
	if [ ! -d "$TARBALLS" ]; then
		mkdir "$TARBALLS"
	fi

	if [ -f "$TARBALLS/$2" ]; then
		reply "    $2...SKIPPED"
	else
		wget -O "$TARBALLS/$2" "$1"
		extract "$2"
	fi
}

extract()
{
	if [ ! -d "$SOURCES" ]; then
		mkdir "$SOURCES"
	fi

	tar -xvzf "$TARBALLS/$1" -C "$SOURCES/"
}

cleandir()
{
	if [ -d "$1" ]; then
		rm -rf "$1" || bail
	fi
	mkdir "$1"
}

patchc()
{
	pushd "$SOURCES/$1" || bail
		patch -p1  -i "$PATCHFILES/$2"
	popd
}

for args in "$@"
do
	if [ "$args" = "--setup" ]; then
		BUILD_GCC=true
		BUILD_GCC_2=true
		BUILD_NEWLIB=true
		BUILD_AUTOCONF=true
		BUILD_BINUTILS=true
		BUILD_AUTOMAKE=true
	elif [ "$args" = "--gcc" ]; then
		BUILD_GCC=true
	elif [ "$args" = "--binutils" ]; then
		BUILD_BINUTILS=true
	elif [ "$args" = "--newlib" ]; then
		BUILD_GCC_2=true
		BUILD_NEWLIB=true
	elif [ "$args" = "--autoconf" ]; then
		BUILD_AUTOCONF=true
	elif [ "$args" = "--automake" ]; then
		BUILD_AUTOMAKE=true
	fi
done

message "Fetching Tarballs..."
fetch "https://ftp.gnu.org/gnu/automake/automake-1.14.tar.gz" "automake-1.14.tar.gz"
fetch "https://ftp.gnu.org/gnu/autoconf/autoconf-2.65.tar.gz" "autoconf-2.65.tar.gz"
fetch "https://ftp.gnu.org/gnu/binutils/binutils-2.26.tar.gz" "binutils-2.26.tar.gz"
fetch "https://ftp.gnu.org/gnu/gcc/gcc-5.3.0/gcc-5.3.0.tar.gz" "gcc-5.3.0.tar.gz"
fetch "ftp://sources.redhat.com/pub/newlib/newlib-1.19.0.tar.gz" "newlib-1.19.0.tar.gz"

patchc "binutils-2.26" "binutils-2.26.diff"
patchc "gcc-5.3.0" "gcc-5.3.0.diff"

message "Building Stuffs..."

if [ ! -d build ]; then
	mkdir build
fi

if [ ! -d $PREFIX ]; then
	mkdir $PREFIX
fi

mkdir build
pushd build || bail

	if $BUILD_AUTOCONF; then
		reply "    Compiling autoconf"
		cleandir "autoconf-native"
		pushd autoconf-native || bail
			$SOURCES/autoconf-2.65/configure --prefix=$PREFIX || bail
			make -j4 || bail
			make -j4 install || bail
		popd
	fi

	if $BUILD_AUTOMAKE; then
		reply "    Compiling automake"
		cleandir "automake-native"
		pushd automake-native || bail
			$SOURCES/automake-1.14/configure --prefix=$PREFIX || bail
			make -j4 || bail
			make -j4 install || bail
		popd
	fi

	if $BUILD_BINUTILS; then
		reply "    Compiling binutils"
		cleandir "binutils-native"
		pushd binutils-native || bail
			$SOURCES/binutils-2.26/configure --prefix=$PREFIX --target=$TARGET || bail
			make -j4 || bail
			make -j4 install || bail
		popd
	fi

	if $BUILD_GCC; then
		reply "    Compiling gcc"
		cleandir "gcc-native"
		pushd $SOURCES/gcc-5.3.0/libstdc++-v3 || bail
			autoconf
		popd
		pushd gcc-native || bail
			$SOURCES/gcc-5.3.0/configure --prefix=$PREFIX --target=$TARGET --disable-nls --without-headers --enable-languages=c,c++ --disable-libssp --with-gnu-as --with-gnu-ld --with-newlib || bail
			make -j4 all-gcc || bail
			make -j4 install-gcc || bail
		popd
	fi

	if $BUILD_NEWLIB; then
		reply "    Compiling newlib"
		cleandir "newlib"
		pushd newlib || bail
			$SOURCES/newlib/configure --target=$TARGET --prefix=$PREFIX || bail
			make -j4 || bail
			make install || bail
		popd
	fi

	if $BUILD_GCC_2; then
		reply "    Compiling gcc again"
		pushd gcc-native || bail
			make -j4 all-target-libstdc++-v3 || bail
			make -j4 install-target-libstdc++-v3 || bail
		popd
	fi

popd

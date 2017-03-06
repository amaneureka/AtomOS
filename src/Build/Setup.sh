#!/usr/bin/env bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

export TARGET=i386-atomos
export PREFIX="$DIR/Local"
export SOURCES="$DIR/Temp"
export TARBALLS="$DIR/../../tarballs"
export PATCHFILES="$DIR/../../toolchain"
export PATH=/usr/bin:$PREFIX/bin:$PATH

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
		mkdir "$TARBALLS" > Setup.log 2>&1
	fi

	if [ -f "$TARBALLS/$2" ]; then
		reply "    $2...SKIPPED"
	else
		wget -O "$TARBALLS/$2" "$1" || bail
		extract "$2"
	fi
}

extract()
{
	if [ ! -d "$SOURCES" ]; then
		mkdir "$SOURCES" > Setup.log 2>&1
	fi

	tar -xvzf "$TARBALLS/$1" -C "$SOURCES/" > Setup.log 2>&1 || bail
}

cleandir()
{
	if [ -d "$1" ]; then
		rm -rf "$1" > Setup.log 2>&1 || bail
	fi
	mkdir "$1"
}

patchc()
{
	pushd "$SOURCES/$1" > Setup.log 2>&1 || bail
		patch -p1  -i "$PATCHFILES/$2" > Setup.log 2>&1
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

if [ ! -d Bin ]; then
	mkdir Bin > Setup.log 2>&1
fi

if [ ! -d $PREFIX ]; then
	mkdir $PREFIX > Setup.log 2>&1
fi

message "Fetching Tarballs..."
fetch "https://ftp.gnu.org/gnu/automake/automake-1.12.tar.gz" "automake-1.12.tar.gz"
fetch "https://ftp.gnu.org/gnu/autoconf/autoconf-2.65.tar.gz" "autoconf-2.65.tar.gz"
fetch "https://ftp.gnu.org/gnu/binutils/binutils-2.26.tar.gz" "binutils-2.26.tar.gz"
fetch "https://ftp.gnu.org/gnu/gcc/gcc-5.3.0/gcc-5.3.0.tar.gz" "gcc-5.3.0.tar.gz"
fetch "ftp://sources.redhat.com/pub/newlib/newlib-1.19.0.tar.gz" "newlib-1.19.0.tar.gz"

patchc "binutils-2.26" "binutils-2.26.diff"
patchc "gcc-5.3.0" "gcc-5.3.0.diff"
patchc "newlib-1.19.0" "newlib-1.19.0.diff"

message "Building Stuffs..."

pushd Bin > Setup.log 2>&1 || bail

	if $BUILD_AUTOCONF; then
		reply "    Compiling autoconf"
		cleandir "autoconf-native"
		pushd autoconf-native > Setup.log 2>&1 || gbail
			$SOURCES/autoconf-2.65/configure --prefix=$PREFIX > Setup.log 2>&1  || bail
			make -j4 > Setup.log 2>&1 || bail
			make -j4 install > Setup.log 2>&1 || bail
		popd
	fi

	if $BUILD_AUTOMAKE; then
		reply "    Compiling automake"
		cleandir "automake-native"
		pushd automake-native > Setup.log 2>&1 || bail
			$SOURCES/automake-1.12/configure --prefix=$PREFIX > Setup.log 2>&1 || bail
			make -j4 > Setup.log 2>&1 || bail
			make -j4 install > Setup.log 2>&1 || bail
		popd
	fi

	if $BUILD_BINUTILS; then
		reply "    Compiling binutils"
		cleandir "binutils-native"
		pushd binutils-native > Setup.log 2>&1 || bail
			$SOURCES/binutils-2.26/configure --prefix=$PREFIX --target=$TARGET > Setup.log 2>&1 || bail
			make -j4 > Setup.log || bail
			make -j4 install > Setup.log || bail
		popd
	fi

	if $BUILD_GCC; then
		reply "    Compiling gcc"
		cleandir "gcc-native"
		pushd $SOURCES/gcc-5.3.0/libstdc++-v3 > Setup.log 2>&1 || bail
			autoconf > Setup.log 2>&1
		popd
		pushd gcc-native > Setup.log 2>&1 || bail
			$SOURCES/gcc-5.3.0/configure --prefix=$PREFIX --target=$TARGET --disable-nls --without-headers --enable-languages=c,c++ --disable-libssp --with-gnu-as --with-gnu-ld --with-newlib > Setup.log 2>&1 || bail
			make -j4 all-gcc > Setup.log || bail
			make -j4 install-gcc > Setup.log 2>&1 || bail
		popd
	fi

	if $BUILD_NEWLIB; then
		reply "    Compiling newlib"
		cleandir "newlib"
		cp -r $PATCHFILES/newlib $SOURCES/newlib-1.19.0 > Setup.log 2>&1 || bail
		pushd $SOURCES/newlib-1.19.0/newlib/libc/sys/atomos > Setup.log 2>&1 || bail
			autoreconf > Setup.log 2>&1 || bail
		popd
		pushd $SOURCES/newlib-1.19.0/newlib/libc/sys > Setup.log 2>&1 || bail
			autoconf > Setup.log 2>&1 || bail
		popd
		pushd newlib || bail
			$SOURCES/newlib-1.19.0/configure --target=$TARGET --prefix=$PREFIX > Setup.log 2>&1 || bail
			make -j4 > Setup.log || bail
			make install > Setup.log 2>&1 || bail
		popd
		pushd $SOURCES/newlib-1.19.0/newlib/libc/sys/atomos > Setup.log 2>&1 || bail
			nasm -felf crti.s -o $PREFIX/$TARGET/lib/crti.o || bail
			nasm -felf crtn.s -o $PREFIX/$TARGET/lib/crtn.o || bail
		popd
	fi

	if $BUILD_GCC_2; then
		reply "    Compiling gcc again"
		pushd gcc-native > Setup.log 2>&1 || bail
			make -j4 all-target-libstdc++-v3 > Setup.log || bail
			make -j4 install-target-libstdc++-v3 > Setup.log 2>&1 || bail
		popd
	fi

popd


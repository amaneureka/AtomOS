#ifndef _SYSCALL_H
#define _SYSCALL_H

#include <_ansi.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <sys/time.h>

#include <stdint.h>
#include <string.h>

/********************************************************************
                        SYSCALL FUNCTIONS
********************************************************************/

#define SYS_exit            1
#define SYS_fork            2
#define SYS_read            3
#define SYS_write           4
#define SYS_open            5
#define SYS_close           6
#define SYS_wait4           7
#define SYS_creat           8
#define SYS_link            9
#define SYS_unlink          10
#define SYS_execv           11
#define SYS_chdir           12
#define SYS_mknod           14
#define SYS_chmod           15
#define SYS_chown           16
#define SYS_lseek           19
#define SYS_getpid          20
#define SYS_isatty          21
#define SYS_fstat           22
#define SYS_time            23
#define SYS_ARG             24
#define SYS_kill            37
#define SYS_stat            38
#define SYS_pipe            42
#define SYS_brk             45
#define SYS_execve          59
#define SYS_gettimeofday    78
#define SYS_truncate        129
#define SYS_ftruncate       130
#define SYS_argc            172
#define SYS_argnlen         173
#define SYS_argn            174
#define SYS_utime           201
#define SYS_wait            202

/********************************************************************
                        HELPER FUNCTIONS
********************************************************************/
#define DEFINE_SYSCALL_0(fn, num) \
    int fn() \
    { \
        int a; \
        asm volatile("int $0x7f" : "=a" (a) : "0" (num)); \
        return a; \
    }

#define DEFINE_SYSCALL_1(fn, num, P1) \
    int fn(P1 p1) \
    { \
        int a; \
        asm volatile("int $0x7f" : "=a" (a) : "0" (num), "b" ((int)p1)); \
        return a; \
    }

#define DEFINE_SYSCALL_2(fn, num, P1, P2) \
    int fn(P1 p1, P2 p2) \
    { \
        int a; \
        asm volatile("int $0x7f" : "=a" (a) : "0" (num), "b" ((int)p1), "c" ((int)p2)); \
        return a; \
    }

#define DEFINE_SYSCALL_3(fn, num, P1, P2, P3) \
    int fn(P1 p1, P2 p2, P3 p3) \
    { \
        int a; \
        asm volatile("int $0x7f" : "=a" (a) : "0" (num), "b" ((int)p1), "c" ((int)p2), "d" ((int)p3)); \
        return a; \
    }

/********************************************************************
                        DECLARATION
********************************************************************/

int _exit(int code);
int close(int file);
int execve(char *name, char **argv, char **env);
int fork();
int fstat(int file, struct stat *st);
int getpid();
int isatty(int file);
int kill(int pid, int sig);
int link(char *old, char *new);
int lseek(int file, int ptr, int dir);
int open(const char *name, int flags, int mode);
int read(int file, char *ptr, int len);
int brk(int incr);
int stat(const char *file, struct stat *st);
int unlink(char *name);
int wait(int *status);
int write(int file, char *ptr, int len);
//int gettimeofday(struct timeval *p, struct timezone *z);

#endif
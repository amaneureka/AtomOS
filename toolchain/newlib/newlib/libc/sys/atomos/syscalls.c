#include <_ansi.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <sys/time.h>

#include <sys/syscalls.h>

//int _exit(int code);
DEFINE_SYSCALL_1(_exit, SYS_exit, int);
//int close(int file);
DEFINE_SYSCALL_1(close, SYS_close, int);
//int execve(char *name, char **argv, char **env);
DEFINE_SYSCALL_3(execve, SYS_execve, char *, char **, char **);
//int fork();
DEFINE_SYSCALL_0(fork, SYS_fork);
//int fstat(int file, struct stat *st);
DEFINE_SYSCALL_2(fstat, SYS_fstat, int, struct stat *);
//int getpid();
DEFINE_SYSCALL_0(getpid, SYS_getpid);
//int isatty(int file);
DEFINE_SYSCALL_1(isatty, SYS_isatty, int);
//int kill(int pid, int sig);
DEFINE_SYSCALL_2(kill, SYS_kill, int, int);
//int link(char *old, char *new);
DEFINE_SYSCALL_2(link, SYS_link, char *, char *);
//int lseek(int file, int ptr, int dir);
DEFINE_SYSCALL_3(lseek, SYS_lseek, int, int, int);
//int open(const char *name, int flags, int mode);
DEFINE_SYSCALL_3(open, SYS_open, const char*, int, int);
//int read(int file, char *ptr, int len);
DEFINE_SYSCALL_3(read, SYS_read, int, char *, int);
//int brk(int incr);
DEFINE_SYSCALL_1(brk, SYS_brk, int);
//int stat(const char *file, struct stat *st);
//DEFINE_SYSCALL_2(stat, SYS_stat, char *, struct stat *);
//int unlink(char *name);
DEFINE_SYSCALL_1(unlink, SYS_unlink, char *);
//int wait(int *status);
DEFINE_SYSCALL_1(wait, SYS_wait, int *);
//int write(int file, char *ptr, int len);
DEFINE_SYSCALL_3(write, SYS_write, int, char *, int);
//int gettimeofday(struct timeval *p, struct timezone *z);
//DEFINE_SYSCALL_2(gettimeofday, SYS_gettimeofday, struct timeval *, void * timezone);

caddr_t
sbrk(int nbytes)
{
	return (caddr_t)brk(nbytes);
}

clock_t
times(struct tms *buf)
{
	return -1;
}

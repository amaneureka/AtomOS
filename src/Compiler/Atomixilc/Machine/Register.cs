/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Architecture Registers
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Machine
{
    public enum Register
    {
        EAX, AX, AH, AL,
        EBX, BX, BH, BL,
        ECX, CX, CH, CL,
        EDX, DX, DH, DL,
        CS, DS, ES, FS,
        GS, SS, ESP, SP,
        EBP, BP, ESI, SI,
        EDI, DI, CR0, CR1,
        CR2, CR3, CR4, XMM0,
        XMM1, XMM2, XMM3, XMM4,
        XMM5, XMM6, XMM7, ST0,
        ST1, ST2, ST3, ST4,
        ST5, ST6, ST7, R0,
        R1, R2, R3, R4,
        R5, R6, R7, R8,
        R9, R10, R11, R12,
        R13, R14, R15
    };
}

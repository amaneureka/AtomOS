/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Registers enum
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Assembler
{
    public enum Registers
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
        ST5, ST6, ST7, r0,
        r1, r2, r3, r4,
        r5, r6, r7, r8,
        r9, r10, r11, r12,
        r13, r14, r15
    };
}

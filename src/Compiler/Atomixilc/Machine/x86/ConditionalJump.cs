/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          x86 Conditional Jumps Listenings
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Machine.x86
{
    public enum ConditionalJump
    {
        JMP, JO, JNO, JS, JNS, JE, JZ, JNE, JNZ, JB, JNAE, JC,
        JNB, JAE, JNC, JBE, JNA, JA, JNBE, JL, JNGE, JGE, JNL,
        JLE, JNG, JG, JNLE, JP, JPE, JNP, JPO, JCXZ, JECXZ,
    };
}

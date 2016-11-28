using System;

namespace Atomixilc.Machine.x86
{
    public enum ConditionalJump
    {
        JMP, JO, JNO, JS, JNS, JE, JZ, JNE, JNZ, JB, JNAE, JC,
        JNB, JAE, JNC, JBE, JNA, JA, JNBE, JL, JNGE, JGE, JNL,
        JLE, JNG, JG, JNLE, JP, JPE, JNP, JPO, JCXZ, JECXZ,
    };
}

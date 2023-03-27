using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Opcode {
    ADD,
    ADDI,
    SUB,
    SUBI,
    MUL,
    DIV,
    MOD,
    COPY,
    COPYI,
    LOAD,
    LOADI,
    STORE,
    CMP,
    CMPI,
    BRANCHE,
    BRANCHG,
    JUMP,
    BREAK,
}

public enum Mode
{
    Release,
    DebugC,
    DebugS,
}
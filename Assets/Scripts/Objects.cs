using System.Collections.Generic;

public enum Opcode {
    NULL,
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
    BRANCHE,
    BRANCHG,
    BRANCHGE,
    JUMP,
    BREAK,
    HALT
}

public enum Mode
{
    RELEASE,
    DEBUGC,
    DEBUGS
}

public readonly struct Instruction
{
    public readonly Opcode Opcode;
    public readonly List<int> Operands;

    public Instruction(Opcode opcode, List<int> operands)
    {
        Opcode = opcode;
        Operands = operands;
    }

    public override string ToString()
    {
        return Opcode + " " + string.Join(' ', Operands);
    }
}
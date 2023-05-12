using System.Collections.Generic;

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
    BRANCHE,
    BRANCHG,
    BRANCHGE,
    JUMP,
    BREAK,
    HALT
}

public struct Instruction
{
    public Opcode Opcode;
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

public struct Result
{
    public int ReservationStationId;
    public int DestinationRegister;
    public int Value;
}
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

public readonly struct Instruction
{
    public readonly Opcode Opcode;
    public readonly int? Destination;
    public readonly List<int> Sources;

    public Instruction(Opcode opcode, int? destination, List<int> sources)
    {
        Opcode = opcode;
        Destination = destination;
        Sources = sources;
    }

    public override string ToString()
    {
        return Opcode + " " + Destination + " " + string.Join(' ', Sources);
    }
}

public struct ProgramSpecification
{
    public readonly Instruction[] Instructions;
    public readonly int[] InitialMemory;
    public readonly ProcessorMode InitialProcessorMode;

    public ProgramSpecification(Instruction[] instructions, int[] initialMemory, ProcessorMode initialProcessorMode)
    {
        Instructions = instructions;
        InitialMemory = initialMemory;
        InitialProcessorMode = initialProcessorMode;
    }
}
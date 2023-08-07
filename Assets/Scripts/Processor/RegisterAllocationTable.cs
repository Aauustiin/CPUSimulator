using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegisterAllocationTable
{
    public readonly int[] Table;
    private readonly Processor _processor;
    
    public RegisterAllocationTable(int size, Processor processor)
    {
        Table = new int[size];
        for (var i = 0; i < size; i++)
        {
            Table[i] = i;
        }

        _processor = processor;
    }

    public Instruction? ConvertInstruction(Instruction instruction)
    {
        var convertedSources = ConvertSources(instruction);

        var potentialNewDestination = _processor.GetAvailableRegister();
        if ((instruction.Destination != null) & (potentialNewDestination == null)) return null;

        if ((instruction.Destination != null) & (potentialNewDestination != null))
            Table[instruction.Destination.Value] = potentialNewDestination.Value;
        
        // We don't wanna convert all the sources, only those that are references to registers
        return new Instruction(instruction.Opcode, 
            instruction.Destination == null ? null : potentialNewDestination,
            convertedSources);
    }

    private List<int> ConvertSources(Instruction instruction)
    {
        return instruction.Opcode switch
        {
            Opcode.ADD => new List<int>() {Table[instruction.Sources[0]], Table[instruction.Sources[1]]},
            Opcode.ADDI => new List<int>() {Table[instruction.Sources[0]], instruction.Sources[1]},
            Opcode.SUB => new List<int>() {Table[instruction.Sources[0]], Table[instruction.Sources[1]]},
            Opcode.SUBI => new List<int>() {Table[instruction.Sources[0]], instruction.Sources[1]},
            Opcode.MUL => new List<int>() {Table[instruction.Sources[0]], Table[instruction.Sources[1]]},
            Opcode.DIV => new List<int>() {Table[instruction.Sources[0]], Table[instruction.Sources[1]]},
            Opcode.MOD => new List<int>() {Table[instruction.Sources[0]], Table[instruction.Sources[1]]},
            Opcode.COPY => new List<int>() {Table[instruction.Sources[0]]},
            Opcode.COPYI => new List<int>() {instruction.Sources[0]},
            Opcode.LOAD => new List<int>() {Table[instruction.Sources[0]]},
            Opcode.LOADI => new List<int>() {instruction.Sources[0]},
            Opcode.STORE => new List<int>() {Table[instruction.Sources[0]], Table[instruction.Sources[1]]},
            Opcode.BRANCHE => new List<int>() {Table[instruction.Sources[0]], Table[instruction.Sources[1]], instruction.Sources[2]},
            Opcode.BRANCHG => new List<int>() {Table[instruction.Sources[0]], Table[instruction.Sources[1]], instruction.Sources[2]},
            Opcode.BRANCHGE => new List<int>() {Table[instruction.Sources[0]], Table[instruction.Sources[1]], instruction.Sources[2]},
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override string ToString()
    {
        return String.Join(' ', Table);
    }
}

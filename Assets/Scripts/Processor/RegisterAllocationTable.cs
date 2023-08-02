using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegisterAllocationTable
{
    private readonly int[] _table;
    private readonly Processor _processor;
    
    public RegisterAllocationTable(int size, Processor processor)
    {
        _table = new int[size];
        for (var i = 0; i < size; i++)
        {
            _table[i] = i;
        }

        _processor = processor;
    }

    public Instruction? ConvertInstruction(Instruction instruction)
    {
        var potentialNewDestination = _processor.GetAvailableRegister();
        if ((instruction.Destination != null) & (potentialNewDestination == null)) return null;

        if ((instruction.Destination != null) & (potentialNewDestination != null))
            _table[instruction.Destination.Value] = potentialNewDestination.Value;
        
        // We don't wanna convert all the sources, only those that are references to registers
        return new Instruction(instruction.Opcode, 
            instruction.Destination == null ? null : potentialNewDestination,
            ConvertSources(instruction));
    }

    public List<int> ConvertSources(Instruction instruction)
    {
        if (DecodeUnit.AllRegOpcodes.Contains(instruction.Opcode) | (instruction.Opcode == Opcode.LOAD))
        {
            return instruction.Sources.Select(source => _table[source]).ToList();
        }

        if (DecodeUnit.RegImmOpcodes.Contains(instruction.Opcode))
        {
            return new List<int> { _table[instruction.Sources[0]], instruction.Sources[1] };
        }

        return instruction.Sources;
    }

    public override string ToString()
    {
        return String.Join(' ', _table);
    }
}

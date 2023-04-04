using System;
using System.Linq;
using UnityEngine;

public class LoadStoreUnit : IExecutionUnit
{
    private readonly Processor _processor;

    private readonly Opcode[] _compatibleOpcodes =
    {
        Opcode.COPY,
        Opcode.COPYI,
        Opcode.LOAD,
        Opcode.LOADI,
        Opcode.STORE,
    };
    
    public LoadStoreUnit(Processor processor)
    {
        _processor = processor;
    }
    
    public void Execute()
    {
        // Try and find an instruction that I can execute
        var instruction = _processor.DecodeExecuteBuffer.Find(
            ins => _compatibleOpcodes.Contains(ins.Opcode)
        );
        bool validInstruction = true;

        switch (instruction.Opcode)
        {
            case Opcode.COPY:
                _processor.Registers[instruction.Operands[0]] = _processor.Registers[instruction.Operands[1]];
                break;
            case Opcode.COPYI:
                _processor.Registers[instruction.Operands[0]] = instruction.Operands[1];
                break;
            case Opcode.LOAD:
                _processor.Registers[instruction.Operands[0]] = _processor.Memory[_processor.Registers[instruction.Operands[1]]];
                break;
            case Opcode.LOADI:
                _processor.Registers[instruction.Operands[0]] = _processor.Memory[instruction.Operands[1]];
                break;
            case Opcode.STORE:
                _processor.Memory[_processor.Registers[instruction.Operands[0]]] = _processor.Registers[instruction.Operands[1]];
                break;
            default:
                validInstruction = false;
                break;
        }

        if (validInstruction)
        {
            _processor.InstructionsExecuted++;
            _processor.DecodeExecuteBuffer.Remove(instruction);
        }
    }
}

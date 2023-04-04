using System;
using System.Linq;
using UnityEngine;

public class IntegerArithmeticUnit : IExecutionUnit
{
    private readonly Processor _processor;
    
    private readonly Opcode[] _compatibleOpcodes =
    {
        Opcode.ADD,
        Opcode.ADDI,
        Opcode.SUB,
        Opcode.SUBI,
        Opcode.MUL,
        Opcode.DIV,
        Opcode.MOD,
    };
    
    public IntegerArithmeticUnit(Processor processor)
    {
        _processor = processor;
    }
    
    public void Execute()
    {
        // Try and find an instruction that I can execute
        var instruction = _processor.DecodeExecuteBuffer.Find(
            ins => _compatibleOpcodes.Contains(ins.Opcode)
        );
        Debug.Log(instruction.Opcode);
        bool validInstruction = true;
            
        switch (instruction.Opcode)
        {
            case Opcode.ADD:
                _processor.Registers[instruction.Operands[0]] = _processor.Registers[instruction.Operands[1]] +
                                                                _processor.Registers[instruction.Operands[2]];
                break;
            case Opcode.ADDI:
                _processor.Registers[instruction.Operands[0]] = _processor.Registers[instruction.Operands[1]] +
                                                                instruction.Operands[2];
                break;
            case Opcode.SUB:
                _processor.Registers[instruction.Operands[0]] = _processor.Registers[instruction.Operands[1]] -
                                                                _processor.Registers[instruction.Operands[2]];
                break;
            case Opcode.SUBI:
                _processor.Registers[instruction.Operands[0]] = _processor.Registers[instruction.Operands[1]] -
                                                                instruction.Operands[2];
                break;
            case Opcode.MUL:
                _processor.Registers[instruction.Operands[0]] = _processor.Registers[instruction.Operands[1]] *
                                                                _processor.Registers[instruction.Operands[2]];
                break;
            case Opcode.DIV:
                _processor.Registers[instruction.Operands[0]] = _processor.Registers[instruction.Operands[1]] /
                                                                _processor.Registers[instruction.Operands[2]];
                break;
            case Opcode.MOD:
                _processor.Registers[instruction.Operands[0]] = _processor.Registers[instruction.Operands[1]] %
                                                                _processor.Registers[instruction.Operands[2]];
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

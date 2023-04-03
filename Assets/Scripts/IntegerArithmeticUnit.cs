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
        Opcode.CMP,
        Opcode.CMPI
    };
    
    public IntegerArithmeticUnit(Processor processor)
    {
        _processor = processor;
    }
    
    public Tuple<Opcode, int, int> Execute()
    {
        // Try and find an instruction that I can execute
        var instruction = _processor.DecodeExecuteBuffer.Find(
            ins => _compatibleOpcodes.Contains(ins.Item1)
        );
        if (instruction == null) return null;
            
        switch (instruction.Item1)
        {
            case Opcode.ADD:
                _processor.Registers[instruction.Item2] += _processor.Registers[instruction.Item3];
                break;
            case Opcode.ADDI:
                _processor.Registers[instruction.Item2] += instruction.Item3;
                break;
            case Opcode.SUB:
                _processor.Registers[instruction.Item2] -= _processor.Registers[instruction.Item3];
                break;
            case Opcode.SUBI:
                _processor.Registers[instruction.Item2] -= instruction.Item3;
                break;
            case Opcode.MUL:
                _processor.Registers[instruction.Item2] *= _processor.Registers[instruction.Item3];
                break;
            case Opcode.DIV:
                _processor.Registers[instruction.Item2] /= _processor.Registers[instruction.Item3];
                break;
            case Opcode.MOD:
                _processor.Registers[instruction.Item2] %= _processor.Registers[instruction.Item3];
                break;
            case Opcode.CMP:
                if (_processor.Registers[instruction.Item2] == _processor.Registers[instruction.Item3])
                {
                    _processor.Registers[instruction.Item2] = 0;
                }
                else if (_processor.Registers[instruction.Item2] > _processor.Registers[instruction.Item3])
                {
                    _processor.Registers[instruction.Item2] = -1;
                }
                else _processor.Registers[instruction.Item2] = 1;
                break;
            case Opcode.CMPI:
                if (_processor.Registers[instruction.Item2] == instruction.Item3)
                {
                    _processor.Registers[instruction.Item2] = 0;
                }
                else if (_processor.Registers[instruction.Item2] > instruction.Item3)
                {
                    _processor.Registers[instruction.Item2] = -1;
                }
                else _processor.Registers[instruction.Item2] = 1;
                break;
            default:
                Debug.Log("LoadStoreUnit tried to execute incompatible instruction.");
                return null;
        }

        _processor.InstructionsExecuted++;
        _processor.DecodeExecuteBuffer.Remove(instruction);
        return instruction;
    }
}

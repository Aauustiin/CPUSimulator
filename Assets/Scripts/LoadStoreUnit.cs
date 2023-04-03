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
    
    public Tuple<Opcode, int, int> Execute()
    {
        // Try and find an instruction that I can execute
        var instruction = _processor.DecodeExecuteBuffer.Find(
            ins => _compatibleOpcodes.Contains(ins.Item1)
            );
        if (instruction == null) return null;

        switch (instruction.Item1)
        {
            case Opcode.COPY:
                _processor.Registers[instruction.Item2] = _processor.Registers[instruction.Item3];
                break;
            case Opcode.COPYI:
                _processor.Registers[instruction.Item2] = instruction.Item3;
                break;
            case Opcode.LOAD:
                _processor.Registers[instruction.Item2] = _processor.Memory[_processor.Registers[instruction.Item3]];
                break;
            case Opcode.LOADI:
                _processor.Registers[instruction.Item2] = _processor.Memory[instruction.Item3];
                break;
            case Opcode.STORE:
                _processor.Memory[_processor.Registers[instruction.Item2]] = _processor.Registers[instruction.Item3];
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

using System;
using System.Linq;
using UnityEngine;

public class BranchUnit : IExecutionUnit
{
    private readonly Processor _processor;
    
    private readonly Opcode[] _compatibleOpcodes =
    {
        Opcode.BRANCHE,
        Opcode.BRANCHG,
        Opcode.JUMP,
        Opcode.BREAK,
        Opcode.HALT,
    };
    
    public BranchUnit(Processor processor)
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
            case Opcode.BRANCHE:
                if (_processor.Registers[instruction.Item2] == 0)
                {
                    _processor.ProgramCounter += instruction.Item3 - 1;
                    _processor.TriggerFlush();
                }
                break;
            case Opcode.BRANCHG:
                if (_processor.Registers[instruction.Item2] == 1)
                {
                    _processor.ProgramCounter += instruction.Item3 - 1;
                    _processor.TriggerFlush();
                }
                break;
            case Opcode.JUMP:
                _processor.ProgramCounter += instruction.Item2 - 1;
                _processor.TriggerFlush();
                break;
            case Opcode.BREAK:
                if (_processor.Mode == Mode.DEBUGC)
                {
                    _processor.Mode = Mode.DEBUGS;
                }
                break;
            case Opcode.HALT:
                _processor.Halt();
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

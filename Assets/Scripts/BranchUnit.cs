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
        Opcode.BRANCHGE,
        Opcode.JUMP,
        Opcode.BREAK,
        Opcode.HALT,
    };
    
    public BranchUnit(Processor processor)
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
            case Opcode.BRANCHE:
                if (instruction.Operands[0] == instruction.Operands[1])
                {
                    _processor.ProgramCounter += instruction.Operands[2];
                    _processor.TriggerFlush();
                }
                break;
            case Opcode.BRANCHG:
                if (instruction.Operands[0] > instruction.Operands[1])
                {
                    _processor.ProgramCounter += instruction.Operands[2];
                    _processor.TriggerFlush();
                }
                break;
            case Opcode.BRANCHGE:
                if (instruction.Operands[0] >= instruction.Operands[1])
                {
                    _processor.ProgramCounter += instruction.Operands[2];
                    _processor.TriggerFlush();
                }
                break;
            case Opcode.JUMP:
                _processor.ProgramCounter += instruction.Operands[0];
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

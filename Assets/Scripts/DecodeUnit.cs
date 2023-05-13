using System;
using System.Linq;

public class DecodeUnit
{
    public (Instruction, int)? Input;
    private ReservationStationData? _output;
    private readonly Processor _processor;

    public DecodeUnit(Processor processor)
    {
        Input = null;
        _output = null;
        _processor = processor;
    }

    public void Decode()
    {
        if (!((Input != null) & (_output != null))) return;
        var instruction = Input.Value.Item1;
        var opcode = instruction.Opcode;
        var convertedInstruction = _processor.RegisterAllocationTable.ConvertInstruction(instruction);

        if (IntegerArithmeticUnit.CompatibleOpcodes.Contains(opcode))
        {
            // Make reservation station entry and reorder buffer entry.
        }
        else if (BranchUnit.CompatibleOpcodes.Contains(opcode))
        {
            // Make reservation station entry.
        }
        else
        {
            // Do whatever it is I have to do for load and store instructions.
            
            // Need to wait for any arguments. - Reservation station!
            // Need to calculate their addresses.
            // Need to wait out any hazards.
        }
        
        
        var destinationRegister = GetDestinationRegister(Input.Value);
        var sourceInformation = GetSourceInformation(Input.Value);
        _output = new ReservationStationData(opcode, destinationRegister, sourceInformation.Item1,
            sourceInformation.Item2);
        Input = null;
    }

    public ReservationStationData? Pop()
    {
        var result = _output;
        _output = null;
        return result;
    }

    public bool IsFree()
    {
        return (_output == null) | (Input == null);
    }

    public bool HasOutput()
    {
        return _output != null;
    }

    private (int?[], int?[]) GetSourceInformation(Instruction instruction)
    {
        int?[] sources, sourceValues;
        switch (instruction.Opcode)
        {
            case Opcode.ADD:
                sources = new int?[]
                {
                    _processor.Scoreboard[instruction.Operands[1]],
                    _processor.Scoreboard[instruction.Operands[2]]
                };
                sourceValues = new int?[]
                {
                    sources[0] == null ? _processor.Registers[instruction.Operands[1]] : null,
                    sources[1] == null ? _processor.Registers[instruction.Operands[2]] : null
                };
                break;
            case Opcode.ADDI:
                sources = new int?[]
                {
                    _processor.Scoreboard[instruction.Operands[1]],
                    null
                };
                sourceValues = new int?[]
                {
                    sources[0] == null ? _processor.Registers[instruction.Operands[1]] : null,
                    instruction.Operands[2]
                };
                break;
            case Opcode.SUB:
                sources = new int?[]
                {
                    _processor.Scoreboard[instruction.Operands[1]],
                    _processor.Scoreboard[instruction.Operands[2]]
                };
                sourceValues = new int?[]
                {
                    sources[0] == null ? _processor.Registers[instruction.Operands[1]] : null,
                    sources[1] == null ? _processor.Registers[instruction.Operands[2]] : null
                };
                break;
            case Opcode.SUBI:
                sources = new int?[]
                {
                    _processor.Scoreboard[instruction.Operands[1]],
                    null
                };
                sourceValues = new int?[]
                {
                    sources[0] == null ? _processor.Registers[instruction.Operands[1]] : null,
                    instruction.Operands[2]
                };
                break;
            case Opcode.MUL:
                sources = new int?[]
                {
                    _processor.Scoreboard[instruction.Operands[1]],
                    _processor.Scoreboard[instruction.Operands[2]]
                };
                sourceValues = new int?[]
                {
                    sources[0] == null ? _processor.Registers[instruction.Operands[1]] : null,
                    sources[1] == null ? _processor.Registers[instruction.Operands[2]] : null
                };
                break;
            case Opcode.DIV:
                sources = new int?[]
                {
                    _processor.Scoreboard[instruction.Operands[1]],
                    _processor.Scoreboard[instruction.Operands[2]]
                };
                sourceValues = new int?[]
                {
                    sources[0] == null ? _processor.Registers[instruction.Operands[1]] : null,
                    sources[1] == null ? _processor.Registers[instruction.Operands[2]] : null
                };
                break;
            case Opcode.MOD:
                sources = new int?[]
                {
                    _processor.Scoreboard[instruction.Operands[1]],
                    _processor.Scoreboard[instruction.Operands[2]]
                };
                sourceValues = new int?[]
                {
                    sources[0] == null ? _processor.Registers[instruction.Operands[1]] : null,
                    sources[1] == null ? _processor.Registers[instruction.Operands[2]] : null
                };
                break;
            case Opcode.COPY:
                sources = new int?[]
                {
                    _processor.Scoreboard[instruction.Operands[1]]
                };
                sourceValues = new int?[]
                {
                    sources[0] == null ? _processor.Registers[instruction.Operands[1]] : null
                };
                break;
            case Opcode.COPYI:
                sources = new int?[]
                {
                    null
                };
                sourceValues = new int?[]
                {
                    instruction.Operands[1]
                };
                break;
            case Opcode.LOAD:
                sources = new int?[]
                {
                    _processor.Scoreboard[instruction.Operands[1]]
                };
                sourceValues = new int?[]
                {
                    sources[0] == null ? _processor.Registers[instruction.Operands[1]] : null
                };
                break;
            case Opcode.LOADI:
                sources = new int?[]
                {
                    null
                };
                sourceValues = new int?[]
                {
                    instruction.Operands[1]
                };
                break;
            case Opcode.STORE:
                sources = new int?[]
                {
                    _processor.Scoreboard[instruction.Operands[1]],
                    _processor.Scoreboard[instruction.Operands[2]]
                };
                sourceValues = new int?[]
                {
                    sources[0] == null ? _processor.Registers[instruction.Operands[1]] : null,
                    sources[1] == null ? _processor.Registers[instruction.Operands[2]] : null
                };
                break;
            case Opcode.BRANCHE:
                sources = new int?[]
                {
                    _processor.Scoreboard[instruction.Operands[1]],
                    _processor.Scoreboard[instruction.Operands[2]]
                };
                sourceValues = new int?[]
                {
                    sources[0] == null ? _processor.Registers[instruction.Operands[1]] : null,
                    sources[1] == null ? _processor.Registers[instruction.Operands[2]] : null
                };
                break;
            case Opcode.BRANCHG:
                sources = new int?[]
                {
                    _processor.Scoreboard[instruction.Operands[1]],
                    _processor.Scoreboard[instruction.Operands[2]]
                };
                sourceValues = new int?[]
                {
                    sources[0] == null ? _processor.Registers[instruction.Operands[1]] : null,
                    sources[1] == null ? _processor.Registers[instruction.Operands[2]] : null
                };
                break;
            case Opcode.BRANCHGE:
                sources = new int?[]
                {
                    _processor.Scoreboard[instruction.Operands[1]],
                    _processor.Scoreboard[instruction.Operands[2]]
                };
                sourceValues = new int?[]
                {
                    sources[0] == null ? _processor.Registers[instruction.Operands[1]] : null,
                    sources[1] == null ? _processor.Registers[instruction.Operands[2]] : null
                };
                break;
            case Opcode.JUMP:
                sources = new int?[]
                {
                    null
                };
                sourceValues = new int?[]
                {
                    instruction.Operands[1]
                };
                break;
            case Opcode.BREAK:
                sources = new int?[] { };
                sourceValues = new int?[] { };
                break;
            case Opcode.HALT:
                sources = new int?[] { };
                sourceValues = new int?[] { };
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return (sources, sourceValues);
    }
}
using System;

public class DecodeUnit
{
    public Instruction? Input;
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
        if ((Input != null) & (_output != null))
        {
            var opcode = Input.Value.Opcode;
            var destinationRegister = GetDestinationRegister(Input.Value);
            var sourceInformation = GetSourceInformation(Input.Value);
            _output = new ReservationStationData(opcode, destinationRegister, sourceInformation.Item1,
                sourceInformation.Item2);
            Input = null;
        }
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
    
    private static int? GetDestinationRegister(Instruction instruction)
    {
        return instruction.Opcode switch
        {
            Opcode.ADD => instruction.Operands[0],
            Opcode.ADDI => instruction.Operands[0],
            Opcode.SUB => instruction.Operands[0],
            Opcode.SUBI => instruction.Operands[0],
            Opcode.MUL => instruction.Operands[0],
            Opcode.DIV => instruction.Operands[0],
            Opcode.MOD => instruction.Operands[0],
            Opcode.COPY => instruction.Operands[0],
            Opcode.COPYI => instruction.Operands[0],
            Opcode.LOAD => instruction.Operands[0],
            Opcode.LOADI => instruction.Operands[0],
            Opcode.STORE => null,
            Opcode.BRANCHE => null,
            Opcode.BRANCHG => null,
            Opcode.BRANCHGE => null,
            Opcode.JUMP => null,
            Opcode.BREAK => null,
            Opcode.HALT => null,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
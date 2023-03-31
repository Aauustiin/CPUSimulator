public class ExecutionUnit
{
    private readonly Processor _processor;

    public ExecutionUnit(Processor processor)
    {
        _processor = processor;
    }

    public void Execute()
    {
        if (_processor.DecodeExecuteBuffer.Count > 0)
        {
            var instruction = _processor.DecodeExecuteBuffer[0];
            _processor.DecodeExecuteBuffer.RemoveAt(0);
            
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
            case Opcode.BRANCHE:
                if (_processor.Registers[instruction.Item2] == 0)
                {
                    _processor.ProgramCounter += instruction.Item3 - 1;
                }
                break;
            case Opcode.BRANCHG:
                if (_processor.Registers[instruction.Item2] == 1)
                {
                    _processor.ProgramCounter += instruction.Item3 - 1;
                }
                break;
            case Opcode.JUMP:
                _processor.ProgramCounter += instruction.Item2 - 1;
                break;
            case Opcode.BREAK:
                if (_processor.Mode == Mode.DebugC)
                {
                    _processor.Mode = Mode.DebugS;
                }
                break;
        }
        }
    }
}

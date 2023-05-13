public class FetchUnit
{
    private readonly Processor _processor;
    private (Instruction, int)? _output;

    public FetchUnit(Processor processor)
    {
        _processor = processor;
        _output = null;
        _processor.BranchMispredict += OnBranchMispredict;
    }

    ~FetchUnit()
    {
        _processor.BranchMispredict -= OnBranchMispredict;
    }

    public void Fetch()
    {
        if (!IsFree() | (_processor.Instructions.Length <= _processor.ProgramCounter)) return;
        
        _output = (_processor.Instructions[_processor.ProgramCounter], _processor.FetchCounter);
        _processor.FetchCounter++;
        
        if (_output.Value.Item1.Opcode == Opcode.JUMP)
        {
            _processor.ProgramCounter += _output.Value.Item1.Sources[0];
            _output = null;
        }
        else if (((_output.Value.Item1.Opcode == Opcode.BRANCHE) | (_output.Value.Item1.Opcode == Opcode.BRANCHG) |
                 (_output.Value.Item1.Opcode == Opcode.BRANCHGE)) & _processor.BranchPredictionUnit.Predict(_output.Value.Item1))
        {
            _processor.ProgramCounter += _output.Value.Item1.Sources[2];
        }
        else _processor.ProgramCounter++;
    }

    public (Instruction, int)? Pop()
    {
        var result = _output;
        _output = null;
        return result;
    }

    private bool IsFree()
    {
        return _output == null;
    }
    
    public bool HasOutput()
    {
        return _output != null;
    }

    private void OnBranchMispredict(int fetchNum)
    {
        if ((_output != null) & (_output.Value.Item2 > fetchNum))
        {
            _output = null;
        }
    }
}

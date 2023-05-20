public class FetchUnit
{
    private readonly Processor _processor;
    private FetchData? _output;

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

        var instruction = _processor.Instructions[_processor.ProgramCounter];

        if (instruction.Opcode == Opcode.JUMP)
        {
            _processor.ProgramCounter = instruction.Sources[0];
            _processor.InstructionsExecuted++;
        }
        else if ((instruction.Opcode == Opcode.BRANCHE) | (instruction.Opcode == Opcode.BRANCHG) |
                 (instruction.Opcode == Opcode.BRANCHGE))
        {
            var prediction = _processor.BranchPredictionUnit.Predict(instruction);
            _output = new FetchData(instruction, _processor.ProgramCounter, _processor.FetchCounter, prediction);
            _processor.ProgramCounter = _output.Value.Instruction.Sources[2];
        }
        else
        {
            _output = new FetchData(instruction, _processor.ProgramCounter, _processor.FetchCounter);
            _processor.ProgramCounter++;
        }
        
        _processor.FetchCounter++;
    }

    public FetchData? Pop()
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
        if ((_output != null) & (_output.Value.FetchNum > fetchNum))
        {
            _output = null;
        }
    }

    public override string ToString()
    {
        return _output.ToString();
    }
}

public struct FetchData
{
    public Instruction Instruction;
    public int ProgramCounter;
    public int FetchNum;
    public bool? Prediction;

    public FetchData(Instruction instruction, int programCounter, int fetchNum)
    {
        Instruction = instruction;
        ProgramCounter = programCounter;
        FetchNum = fetchNum;
        Prediction = null;
    }
    
    public FetchData(Instruction instruction, int programCounter, int fetchNum, bool prediction)
    {
        Instruction = instruction;
        ProgramCounter = programCounter;
        FetchNum = fetchNum;
        Prediction = prediction;
    }

    public override string ToString()
    {
        return "Instruction: " + Instruction + ", PC: " + ProgramCounter + ", FetchNum: " + FetchNum +
            ", Prediction: " + Prediction;
    }
}
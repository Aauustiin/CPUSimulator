public class FetchUnit
{
    private readonly Processor _processor;
    private Instruction? _output;

    public FetchUnit(Processor processor)
    {
        _processor = processor;
        _output = null;
    }

    public void Fetch()
    {
        if (IsFree() && (_processor.Instructions.Length > _processor.ProgramCounter))
        {
            _output = _processor.Instructions[_processor.ProgramCounter];
            _processor.ProgramCounter++;
        }
    }

    public Instruction? Pop()
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
}

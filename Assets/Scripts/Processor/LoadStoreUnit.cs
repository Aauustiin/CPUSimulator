using System;
using System.Collections.Generic;

public class LoadStoreUnit : IExecutionUnit
{
    private ReservationStationData? _input;
    private readonly Processor _processor;
    private int _cyclesToWait;

    private static readonly Opcode[] CompatibleOpcodes =
    {
        Opcode.LOAD,
        Opcode.LOADI,
        Opcode.STORE,
    };
    
    public IEnumerable<Opcode> GetCompatibleOpcodes()
    {
        return CompatibleOpcodes;
    }

    public void Execute()
    {
        if (_input == null) return;

        if (_cyclesToWait > 0)
        {
            _cyclesToWait--;
            return;
        }
        
        switch(_input.Value.Opcode)
        {
            case Opcode.LOAD:
                _processor.ReorderBuffer.Update(_input.Value.FetchNum, 
                    _processor.Memory[_input.Value.SourceValues[0].Value],
                    null);
                break;
            case Opcode.LOADI:
                _processor.ReorderBuffer.Update(_input.Value.FetchNum, 
                    _processor.Memory[_input.Value.SourceValues[0].Value],
                    null);
                break;
            case Opcode.STORE:
                _processor.ReorderBuffer.Update(_input.Value.FetchNum, _input.Value.SourceValues[1], _input.Value.SourceValues[0]);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _input = null;
    }

    private void OnBranchMispredict(int fetchNum)
    {
        if ((_input != null) && (fetchNum < _input.Value.FetchNum))
        {
            _input = null;
            _cyclesToWait = 0;
        }
    }
    
    public void SetInput(ReservationStationData data)
    {
        _input = data;
        _cyclesToWait = 0;
        //_cyclesToWait = 10;
    }
    
    public bool IsFree()
    {
        return _input == null;
    }
    
    public LoadStoreUnit(Processor processor)
    {
        _processor = processor;
        _processor.BranchMispredict += OnBranchMispredict;
    }

    ~LoadStoreUnit()
    {
        _processor.BranchMispredict -= OnBranchMispredict;
    }

    public override string ToString()
    {
        if (_input != null)
            return "Load Store Unit: " + _input + ", Cycles to Wait: " + _cyclesToWait;

        return "Load Store Unit: Empty";
    }
}
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
                _processor.ReorderBuffer.Update(_input.Value.FetchNum, _input.Value.SourceValues[0], null);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void SetInput(ReservationStationData data)
    {
        _input = data;
        _cyclesToWait = 10;
    }
    
    public bool IsFree()
    {
        return _input == null;
    }
    
    public LoadStoreUnit(Processor processor)
    {
        _processor = processor;
    }

    public override string ToString()
    {
        return _input + ", Cycles to Wait: " + _cyclesToWait;
    }
}
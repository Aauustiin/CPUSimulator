using System;
using System.Collections.Generic;

public class BranchUnit : IExecutionUnit
{
    private ReservationStationData? _input;
    private readonly Processor _processor;
    
    private static readonly Opcode[] CompatibleOpcodes =
    {
        Opcode.BRANCHE,
        Opcode.BRANCHG,
        Opcode.BRANCHGE,
    };
    
    public IEnumerable<Opcode> GetCompatibleOpcodes()
    {
        return CompatibleOpcodes;
    }

    public void Execute()
    {
        if (_input == null) return;

        bool branch = _input.Value.Opcode switch
        {
            Opcode.BRANCHE => _input.Value.SourceValues[0].Value == _input.Value.SourceValues[1].Value,
            Opcode.BRANCHG => _input.Value.SourceValues[0].Value > _input.Value.SourceValues[1].Value,
            Opcode.BRANCHGE => _input.Value.SourceValues[0].Value >= _input.Value.SourceValues[1].Value,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (branch & !_input.Value.Prediction.Value)
        {
            _processor.TriggerBranchMispredict(_input.Value.FetchNum);
            _processor.ProgramCounter = _input.Value.SourceValues[0].Value;
            // Need to do something to the reorder buffer so it knows it can commit
        }
        else if (!branch & _input.Value.Prediction.Value)
        {
            _processor.TriggerBranchMispredict(_input.Value.FetchNum);
            _processor.ProgramCounter = _input.Value.ProgramCounter;
        }

        _processor.ReorderBuffer.Update(_input.Value.FetchNum, branch ? 1 : 0, null);
        _input = null;
    }
    
    public void SetInput(ReservationStationData data)
    {
        _input = data;
    }
    
    public bool IsFree()
    {
        return _input == null;
    }
    
    public BranchUnit(Processor processor)
    {
        _processor = processor;
    }

    public override string ToString()
    {
        return _input.ToString();
    }
}

using System;
using System.Collections.Generic;

public class BranchUnit : IExecutionUnit
{
    private ReservationStationData? _input;
    private readonly Processor _processor;

    public static readonly Opcode[] CompatibleOpcodes =
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

        _processor.BranchInstructions++;

        bool branch = _input.Value.Opcode switch
        {
            Opcode.BRANCHE => _input.Value.SourceValues[0].Value == _input.Value.SourceValues[1].Value,
            Opcode.BRANCHG => _input.Value.SourceValues[0].Value > _input.Value.SourceValues[1].Value,
            Opcode.BRANCHGE => _input.Value.SourceValues[0].Value >= _input.Value.SourceValues[1].Value,
            _ => throw new ArgumentOutOfRangeException()
        };

        bool mispredict = false;
        if (branch & !_input.Value.Prediction.Value)
        {
            _processor.TriggerBranchMispredict(_input.Value.FetchNum);
            _processor.ProgramCounter = _input.Value.SourceValues[2].Value;
            mispredict = true;
        }
        else if (!branch & _input.Value.Prediction.Value)
        {
            _processor.TriggerBranchMispredict(_input.Value.FetchNum);
            _processor.ProgramCounter = _input.Value.ProgramCounter + 1;
            mispredict = true;
        }
        if (!mispredict) _processor.ReorderBuffer.Update(_input.Value.FetchNum, branch ? 1 : 0, null);
        
        _processor.BranchPredictionUnit.Notify(_input.Value.Prediction.Value, branch);
        
        _input = null;
    }

    private void OnBranchMispredict(int fetchNum)
    {
        if ((_input != null) && (fetchNum < _input.Value.FetchNum))
        {
            _input = null;
        }
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
        _processor.BranchMispredict += OnBranchMispredict;
    }

    ~BranchUnit()
    {
        _processor.BranchMispredict -= OnBranchMispredict;
    }

    public override string ToString()
    {
        if (_input != null)
            return "Branch Unit: " + _input.ToString();

        return "Branch Unit:  Empty";
    }
}

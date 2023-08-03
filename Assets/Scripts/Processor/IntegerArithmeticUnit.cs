using System;
using System.Collections.Generic;

public class IntegerArithmeticUnit : IExecutionUnit
{
    private readonly Processor _processor;
    private ReservationStationData? _input;
    private int _cyclesToWait;
    
    private static readonly Opcode[] CompatibleOpcodes =
    {
        Opcode.ADD,
        Opcode.ADDI,
        Opcode.SUB,
        Opcode.SUBI,
        Opcode.MUL,
        Opcode.DIV,
        Opcode.MOD,
        Opcode.COPY,
        Opcode.COPYI
    };

    public IEnumerable<Opcode> GetCompatibleOpcodes()
    {
        return CompatibleOpcodes;
    }
    
    public void Execute()
    {
        if (_input == null) return;

        // Wait the right number of cycles depending on what your instruction is.
        if (_cyclesToWait > 0)
        {
            _cyclesToWait--;
            return;
        }
        
        // Do the calculation on your input.
        var result = _input.Value.Opcode switch
        {
            Opcode.ADD => _input.Value.SourceValues[0] + _input.Value.SourceValues[1],
            Opcode.ADDI => _input.Value.SourceValues[0] + _input.Value.SourceValues[1],
            Opcode.SUB => _input.Value.SourceValues[0] - _input.Value.SourceValues[1],
            Opcode.SUBI => _input.Value.SourceValues[0] - _input.Value.SourceValues[1],
            Opcode.MUL => _input.Value.SourceValues[0] * _input.Value.SourceValues[1],
            Opcode.DIV => _input.Value.SourceValues[0] / _input.Value.SourceValues[1],
            Opcode.MOD => _input.Value.SourceValues[0] % _input.Value.SourceValues[1],
            Opcode.COPY => _input.Value.SourceValues[0],
            Opcode.COPYI => _input.Value.SourceValues[0],
            _ => throw new ArgumentOutOfRangeException()
        };

        // Update reorder buffer entry value.
        _processor.ReorderBuffer.Update(_input.Value.FetchNum, result, null);
        
        // Clear input.
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

        //_cyclesToWait = data.Opcode switch
        //{
        //    Opcode.ADD => 0,
        //    Opcode.ADDI => 0,
        //    Opcode.SUB => 0,
        //    Opcode.SUBI => 0,
        //    Opcode.MUL => 3,
        //    Opcode.DIV => 4,
        //    Opcode.MOD => 4,
        //    Opcode.COPY => 0,
        //    Opcode.COPYI => 0,
        //    _ => throw new ArgumentOutOfRangeException()
        //};
    }
    
    public bool IsFree()
    {
        return _input == null;
    }

    public IntegerArithmeticUnit(Processor processor)
    {
        _processor = processor;
        _processor.BranchMispredict += OnBranchMispredict;
    }
    
    ~IntegerArithmeticUnit()
    {
        _processor.BranchMispredict -= OnBranchMispredict;
    }

    public override string ToString()
    {
        if (_input != null)
            return "Integer Arithmetic Unit: " + _input + ", Cycles to Wait: " + _cyclesToWait;
        return "Integer Arithmetic Unit: Empty";
    }
}

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
        Opcode.JUMP,
        Opcode.BREAK,
        Opcode.HALT,
    };
    
    public IEnumerable<Opcode> GetCompatibleOpcodes()
    {
        return CompatibleOpcodes;
    }

    public void Execute()
    {
        // Do execution stuff!
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
}

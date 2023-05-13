using System.Linq;

public class RegisterAllocationTable
{
    private int[] _table;
    private Processor _processor;
    
    public RegisterAllocationTable(int size, Processor processor)
    {
        _table = new int[] { };
        for (var i = 0; i < size; i++)
        {
            _table[i] = i;
        }

        _processor = processor;
    }

    public Instruction? ConvertInstruction(Instruction instruction)
    {
        var potentialNewDestination = _processor.GetAvailableRegister();
        if ((instruction.Destination != null) & (potentialNewDestination == null)) return null;
        
        return new Instruction(instruction.Opcode, 
            instruction.Destination == null ? null : potentialNewDestination,
            instruction.Sources.Select(source => _table[source]).ToList());
    }
}

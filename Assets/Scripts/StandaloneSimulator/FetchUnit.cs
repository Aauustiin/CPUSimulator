using System;
using System.Collections.Generic;

public class FetchUnit
{
    private readonly Processor _processor;
    
    public FetchUnit(Processor processor)
    {
        _processor = processor;
    }

    public List<Tuple<Opcode, int, int>> Fetch()
    {
        var instructions = new List<Tuple<Opcode, int, int>>();
        for (int i = 0; i < _processor.Pipelines; i++)
        {
            if (_processor.Instructions.Length > _processor.ProgramCounter)
            {
                instructions.Add(_processor.Instructions[_processor.ProgramCounter]);
                _processor.ProgramCounter++;
            }
        }
        return instructions;
    }
}

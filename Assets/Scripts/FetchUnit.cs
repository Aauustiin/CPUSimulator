using System;
using System.Collections.Generic;

public class FetchUnit
{
    private readonly Processor _processor;
    public readonly List<Tuple<Opcode, int, int>> OutputBuffer;
    
    public FetchUnit(Processor processor)
    {
        _processor = processor;
        OutputBuffer = new List<Tuple<Opcode, int, int>>();
        _processor.Flush += OnFlush;
    }

    ~FetchUnit()
    {
        _processor.Flush -= OnFlush;
    }

    public void Fetch()
    {
        for (int i = 0; i < _processor.Pipelines; i++)
        {
            if (_processor.Instructions.Length > _processor.ProgramCounter)
            {
                OutputBuffer.Add(_processor.Instructions[_processor.ProgramCounter]);
                _processor.ProgramCounter++;
            }
        }
    }

    private void OnFlush()
    {
        OutputBuffer.Clear();
    }
}

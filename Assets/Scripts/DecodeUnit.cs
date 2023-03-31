using System;
using System.Collections.Generic;

public class DecodeUnit
{
    private readonly Processor _processor;

    public DecodeUnit(Processor processor)
    {
        _processor = processor;
    }

    public List<Tuple<Opcode, int, int>> Decode()
    {
        for (int i = 0; i < _processor.Pipelines; i++)
        {
            _processor.FetchDecodeBuffer[]
            if (_fetchDecodeBuffer.TryPop(out Tuple<Opcode, int, int> instruction))
            {
                _decodeExecuteBuffer.Push(instruction);
            }
        }
    }
}

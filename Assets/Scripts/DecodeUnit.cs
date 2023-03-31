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
        List<Tuple<Opcode, int, int>> decodedInstructions = new List<Tuple<Opcode, int, int>>();
        for (int i = 0; i < _processor.Pipelines; i++)
        {
            if (_processor.FetchDecodeBuffer.Count > 0)
            {
                var instruction = _processor.FetchDecodeBuffer[0];
                decodedInstructions.Add(instruction);
                _processor.FetchDecodeBuffer.RemoveAt(0);
            }
        }
        return decodedInstructions;
    }
}

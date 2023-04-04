using System;
using System.Collections.Generic;

public class DecodeUnit
{
    private readonly Processor _processor;
    public readonly List<Instruction> OutputBuffer;

    public DecodeUnit(Processor processor)
    {
        _processor = processor;
        OutputBuffer = new List<Instruction>();
        _processor.Flush += OnFlush;
    }

    ~DecodeUnit()
    {
        _processor.Flush -= OnFlush;
    }

    public void Decode()
    {
        for (int i = 0; i < _processor.Pipelines; i++)
        {
            if (_processor.FetchDecodeBuffer.Count > 0)
            {
                var instruction = _processor.FetchDecodeBuffer[0];
                OutputBuffer.Add(instruction);
                _processor.FetchDecodeBuffer.RemoveAt(0);
            }
        }
    }

    private void OnFlush()
    {
        OutputBuffer.Clear();
    }
}

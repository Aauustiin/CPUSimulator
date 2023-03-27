using System;
using System.Collections.Generic;

public class DecodeUnit
{
    private Stack<Tuple<Opcode, int, int>> _fetchDecodeBuffer;
    private Stack<Tuple<Opcode, int, int>> _decodeExecuteBuffer;
    private int _pipelines;

    public DecodeUnit(Stack<Tuple<Opcode, int, int>> fetchDecodeBuffer,
        Stack<Tuple<Opcode, int, int>> decodeExecuteBuffer,
        int pipelines)
    {
        _fetchDecodeBuffer = fetchDecodeBuffer;
        _decodeExecuteBuffer = decodeExecuteBuffer;
        _pipelines = pipelines;
        
        EventManager.Tick += Decode;
    }

    ~DecodeUnit()
    {
        EventManager.Tick -= Decode;
    }

    private void Decode()
    {
        for (int i = 0; i < _pipelines; i++)
        {
            if (_fetchDecodeBuffer.TryPop(out Tuple<Opcode, int, int> instruction))
            {
                _decodeExecuteBuffer.Push(instruction);
            }
        }
    }
}

using System;
using System.Collections.Generic;

public class Processor
{
    private int _pipelines;
    
    private FetchUnit _fetchUnit;
    private DecodeUnit _decodeUnit;
    private List<ExecutionUnit> _executionUnits;

    private Stack<Tuple<Opcode, int, int>> _decodeExecuteBuffer;
    private Stack<Tuple<Opcode, int, int>> _fetchDecodeBuffer;
    
    private int[] _registers;
    private int[] _memory;
    private Tuple<Opcode, int, int>[] _instructions;

    private int _programCounter;
    private int _cycle;

    private Mode _mode;

    private void Start()
    {
        _fetchDecodeBuffer = new Stack<Tuple<Opcode, int, int>>();
        _decodeExecuteBuffer = new Stack<Tuple<Opcode, int, int>>();
        
        _fetchUnit = new FetchUnit();
        _decodeUnit = new DecodeUnit(_fetchDecodeBuffer, _decodeExecuteBuffer, _pipelines);
        
    }
}

using System;
using System.Collections.Generic;

public class Processor
{
    public int Pipelines;

    private FetchUnit _fetchUnit;
    private DecodeUnit _decodeUnit;
    private List<ExecutionUnit> _executionUnits;

    public List<Tuple<Opcode, int, int>> FetchDecodeBuffer;
    public List<Tuple<Opcode, int, int>> DecodeExecuteBuffer;

    public int[] Registers;
    public int[] Memory;
    public Tuple<Opcode, int, int>[] Instructions;

    public int ProgramCounter;
    private int _cycle;

    public Mode Mode;

    public Processor(int pipelines)
    {
        Pipelines = pipelines;
        
        _fetchUnit = new FetchUnit(this);
        _decodeUnit = new DecodeUnit(this);
        _executionUnits = new List<ExecutionUnit>();
        for (int i = 0; i < Pipelines; i++)
        {
            _executionUnits.Add(new ExecutionUnit(this));
        }
        
        FetchDecodeBuffer = new List<Tuple<Opcode, int, int>>();
        DecodeExecuteBuffer = new List<Tuple<Opcode, int, int>>();
    }

    private void Tick()
    {
        var fetchedInstructions = _fetchUnit.Fetch();
        var decodedInstructions =
    }
}

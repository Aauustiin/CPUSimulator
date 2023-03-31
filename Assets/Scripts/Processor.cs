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

    private Mode _mode;

    private void Start()
    {
        FetchDecodeBuffer = new List<Tuple<Opcode, int, int>>();
        DecodeExecuteBuffer = new List<Tuple<Opcode, int, int>>();
        
        _fetchUnit = new FetchUnit(this);
        //_decodeUnit = new DecodeUnit();
    }

    private void Tick()
    {
        var fetchedInstructions = _fetchUnit.Fetch();
        var decodedInstructions =
    }
}

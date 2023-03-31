using System;
using System.Collections.Generic;
using UnityEngine;

public class Processor
{
    public readonly int Pipelines;

    private readonly FetchUnit _fetchUnit;
    private readonly DecodeUnit _decodeUnit;
    private readonly List<ExecutionUnit> _executionUnits;

    public readonly List<Tuple<Opcode, int, int>> FetchDecodeBuffer;
    public readonly List<Tuple<Opcode, int, int>> DecodeExecuteBuffer;

    public readonly int[] Registers;
    public readonly int[] Memory;
    public readonly Tuple<Opcode, int, int>[] Instructions;

    public int ProgramCounter;
    private int _cycle;
    public int InstructionsExecuted;
    private bool _finished;

    public Mode Mode;

    public Processor(int pipelines, int registers, int memory, Tuple<Opcode, int, int>[] instructions, Mode mode)
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

        Registers = new int[registers];
        Memory = new int[memory];
        Instructions = instructions;
        
        ProgramCounter = 0;
        _cycle = 0;
        InstructionsExecuted = 0;
        _finished = false;

        Mode = mode;

        EventManager.Tick += OnTick;
        EventManager.TriggerTick();
    }

    ~Processor()
    {
        EventManager.Tick -= OnTick;
    }

    private void OnTick()
    {
        if (_finished) return;
        
        var fetchedInstructions = _fetchUnit.Fetch();
        var decodedInstructions = _decodeUnit.Decode();
        foreach (var eUnit in _executionUnits)
        {
            eUnit.Execute();
        }
        FetchDecodeBuffer.AddRange(fetchedInstructions);
        DecodeExecuteBuffer.AddRange(decodedInstructions);
        _cycle++;

        if (Mode == Mode.DEBUGS)
        {
            Debug.Log("Registers: " + Registers);
            Debug.Log("Memory: " + Memory);
        }
        else EventManager.TriggerTick();
    }

    public void Halt()
    {
        _finished = true;

        Debug.Log("Finished!");
        Debug.Log("Registers: " + Registers);
        Debug.Log("Memory: " + Memory);
        Debug.Log("Instructions Executed: " + InstructionsExecuted);
        Debug.Log("Cycles Taken: " + _cycle);
        Debug.Log("Instructions Per Cycle (IPC): " + (InstructionsExecuted/_cycle).ToString("N2"));
    }
}

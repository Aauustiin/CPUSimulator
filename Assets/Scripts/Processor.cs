using System;
using System.Collections.Generic;
using UnityEngine;

public class Processor
{
    public readonly int Pipelines;

    private readonly FetchUnit _fetchUnit;
    private readonly DecodeUnit _decodeUnit;
    private readonly List<ExecutionUnit> _executionUnits;

    // Could experiment with making these buffers a fixed size
    public List<Tuple<Opcode, int, int>> FetchDecodeBuffer;
    public List<Tuple<Opcode, int, int>> DecodeExecuteBuffer;

    public readonly int[] Registers;
    public int[] Memory;
    public Tuple<Opcode, int, int>[] Instructions;

    public int ProgramCounter;
    private int _cycle;
    public int InstructionsExecuted;
    private bool _finished;

    public Mode Mode;
    
    public static event Action Tick;

    public Processor(int pipelines, int registers)
    {
        Pipelines = pipelines;
        
        _fetchUnit = new FetchUnit(this);
        _decodeUnit = new DecodeUnit(this);
        _executionUnits = new List<ExecutionUnit>();
        for (int i = 0; i < Pipelines; i++)
        {
            _executionUnits.Add(new ExecutionUnit(this));
        }

        Registers = new int[registers];

        Tick += OnTick;
    }

    ~Processor()
    {
        Tick -= OnTick;
    }
    
    public void Process(int[] memory, Tuple<Opcode, int, int>[] instructions, Mode mode)
    {
        FetchDecodeBuffer = new List<Tuple<Opcode, int, int>>();
        DecodeExecuteBuffer = new List<Tuple<Opcode, int, int>>();
        
        Array.Clear(Registers, 0, Registers.Length);
        Memory = memory;
        Instructions = instructions;
        
        ProgramCounter = 0;
        _cycle = 0;
        InstructionsExecuted = 0;
        _finished = false;

        Mode = mode;
        
        TriggerTick();
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
            Debug.Log("Registers: " + string.Join(',', Registers));
            Debug.Log("Memory: " + string.Join(',', Memory));
        }
        else TriggerTick();
    }

    public void Halt()
    {
        _finished = true;

        Debug.Log("Finished!");
        Debug.Log("Registers: " + string.Join(',', Registers));
        Debug.Log("Memory: " + string.Join(',', Memory));
        Debug.Log("Instructions Executed: " + InstructionsExecuted);
        Debug.Log("Cycles Taken: " + _cycle);
        Debug.Log("Instructions Per Cycle (IPC): " + ((float)InstructionsExecuted/_cycle).ToString("N2"));
    }
    
    public void TriggerTick()
    {
        Tick?.Invoke();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Processor
{
    private readonly FetchUnit[] _fetchUnits;
    private readonly DecodeUnit[] _decodeUnits;
    public readonly IExecutionUnit[] ExecutionUnits;

    public readonly IBranchPredictionUnit BranchPredictionUnit;
    public readonly ReservationStation[] ReservationStations;
    public readonly ReorderBuffer ReorderBuffer;

    public readonly int[] Registers;
    public readonly RegisterAllocationTable RegisterAllocationTable;
    
    public int[] Memory;
    public Instruction[] Instructions;

    public int ProgramCounter;
    private int _cycle;
    public int FetchCounter;
    public int InstructionsExecuted;
    private bool _finished;

    public ProcessorMode ProcessorMode;

    public Processor(ProcessorSpecification processorSpecification)
    {
        _fetchUnits = new FetchUnit[processorSpecification.NumFetchUnits];
        for (var i = 0; i < processorSpecification.NumFetchUnits; i++)
        {
            _fetchUnits[i] = new FetchUnit(this);
        }
        
        _decodeUnits = new DecodeUnit[processorSpecification.NumDecodeUnits];
        for (var i = 0; i < processorSpecification.NumDecodeUnits; i++)
        {
            _decodeUnits[i] = new DecodeUnit(this);
        }

        var tempExecutionUnits = new List<IExecutionUnit>();
        
        for (var i = 0; i < processorSpecification.NumIntegerArithmeticUnits; i++)
        {
            tempExecutionUnits.Add(new IntegerArithmeticUnit(this));
        }

        for (var i = 0; i < processorSpecification.NumBranchUnits; i++)
        {
            tempExecutionUnits.Add(new BranchUnit(this));
        }

        for (var i = 0; i < processorSpecification.NumLoadStoreUnits; i++)
        {
            tempExecutionUnits.Add(new LoadStoreUnit(this));
        }

        ExecutionUnits = tempExecutionUnits.ToArray();

        ReservationStations = new ReservationStation[processorSpecification.NumReservationStations];
        for (var i = 0; i < processorSpecification.NumReservationStations; i++)
        {
            ReservationStations[i] = new ReservationStation(this);
        }

        ReorderBuffer = new ReorderBuffer(processorSpecification.ReorderBufferSize, this);

        Registers = new int[processorSpecification.NumPhysicalRegisters];
        RegisterAllocationTable = new RegisterAllocationTable(processorSpecification.NumArchitecturalRegisters, this);

        BranchPredictionUnit = processorSpecification.DynamicBranchPredictor
            ? new DynamicBranchPredictor()
            : new StaticBranchPredictor();

        ProcessorMode = processorSpecification.InitialProcessorMode;

        EventManager.Tick += OnTick;
    }

    ~Processor()
    {
        EventManager.Tick -= OnTick;
    }

    public void Process(ProgramSpecification programSpecification)
    {
        Instructions = programSpecification.Instructions;
        Memory = programSpecification.InitialMemory;
        EventManager.TriggerTick();
    }

    private void OnTick()
    {
        if (_finished) return;
        
        // Step 1: Process the current data.

        foreach (var fetchUnit in _fetchUnits)
        {
            fetchUnit.Fetch();
        }

        foreach (var decodeUnit in _decodeUnits)
        {
            decodeUnit.Decode();
        }

        foreach (var executionUnit in ExecutionUnits)
        {
            executionUnit.Execute();
        }

        // Step 2: Assign new data.

        // Ready reservation stations will issue to free execution units.
        foreach (var station in ReservationStations)
        {
            if (station.GetState() == ReservationStationState.READY) station.Issue();
        }

        //Decode units will send their stuff to the reorder buffer and the reservation stations already.

        // Assign fetched instructions to free decode units.
        var fullFetchUnits = _fetchUnits.Where(fetchUnit => fetchUnit.HasOutput()).ToArray();
        var emptyDecodeUnits = _decodeUnits.Where(decodeUnit => decodeUnit.IsFree()).ToArray();
        var i = 0;
        while ((i < fullFetchUnits.Count()) & (i < emptyDecodeUnits.Count()))
        {
            emptyDecodeUnits[i].Input = fullFetchUnits[i].Pop();
            i++;
        }

        if (ProcessorMode == ProcessorMode.DEBUGS)
        {
            Debug.Log("Cycle: " + _cycle);
            
            Debug.Log("FETCH UNITS");
            foreach (var fetchUnit in _fetchUnits)
            {
                Debug.Log(fetchUnit);
            }

            Debug.Log("DECODE UNITS");
            foreach (var decodeUnit in _decodeUnits)
            {
                Debug.Log(decodeUnit);
            }

            Debug.Log("EXECUTION UNITS");
            foreach (var executionUnit in ExecutionUnits)
            {
                Debug.Log(executionUnit);
            }

            Debug.Log("BRANCH PREDICTION UNIT");
            Debug.Log(BranchPredictionUnit.ToString());

            Debug.Log("RESERVATION STATIONS");
            foreach (var station in ReservationStations)
            {
                Debug.Log(station);
            }

            Debug.Log("REORDER BUFFER");
            Debug.Log(ReorderBuffer);

            Debug.Log("REGISTERS");
            Debug.Log(String.Join(' ', Registers));

            Debug.Log("REGISTER ALLOCATION TABLE");
            Debug.Log(RegisterAllocationTable);

            Debug.Log("MEMORY");
            Debug.Log(String.Join(' ', Memory));

            Debug.Log("STATS");
            Debug.Log("Program Counter: " + ProgramCounter + ", Fetch Counter: " + FetchCounter + ", Instructions Executed: " + InstructionsExecuted);
        }
        
        _cycle++;
        
        // If everything is empty, and the PC is at the end, you're done.
        if (_fetchUnits.All(fetchUnit => !fetchUnit.HasOutput()) &
            _decodeUnits.All(decodeUnit => decodeUnit.IsFree()) &
            ExecutionUnits.All(executionUnit => executionUnit.IsFree()) &
            ReservationStations.All(reservationStation =>
                reservationStation.GetState() == ReservationStationState.FREE) &
            ReorderBuffer.IsEmpty() &
            (ProgramCounter >= Instructions.Length))
        {
            _finished = true;
            
            Debug.Log("Finished! " + String.Join(' ', Registers));
        }

        if (_cycle > 10000)
        {
            _finished = true;
            Debug.Log("Finished! " + String.Join(' ', Registers));
        }
        
        if ((ProcessorMode == ProcessorMode.DEBUGC) | (ProcessorMode == ProcessorMode.RELEASE)) EventManager.TriggerTick();
    }

    public event System.Action<int> BranchMispredict;

    public void TriggerBranchMispredict(int fetchNum)
    {
        BranchMispredict?.Invoke(fetchNum);
    }

    public int? GetAvailableRegister()
    {
        var potentialRegisters = new List<int>();
        for (var i = 0; i < Registers.Length; i++)
        {
            potentialRegisters.Add(i);
        }
        foreach (var entry in ReorderBuffer.Entries)
        {
            var destination = entry.GetDestination();
            if (destination != null) potentialRegisters.Remove(entry.GetDestination().Value);
        }

        if (potentialRegisters.Count == 0) return null;
        return potentialRegisters[0];
    }

    public int? GetAvailableReservationStation()
    {
        var result = Array.FindIndex(ReservationStations, station => station.GetState() == ReservationStationState.FREE);
        return result == -1 ? null : result;
    }
}

public struct ProcessorSpecification
{
    public readonly int NumFetchUnits;
    public readonly int NumDecodeUnits;
    public readonly int NumIntegerArithmeticUnits;
    public readonly int NumBranchUnits;
    public readonly int NumLoadStoreUnits;
    public readonly int NumReservationStations;
    public readonly int NumPhysicalRegisters;
    public readonly int NumArchitecturalRegisters;
    public readonly int ReorderBufferSize;
    public readonly bool DynamicBranchPredictor;
    public readonly ProcessorMode InitialProcessorMode;

    public ProcessorSpecification(
        int numFetchUnits, 
        int numDecodeUnits, 
        int numIntegerArithmeticUnits,
        int numBranchUnits, 
        int numLoadStoreUnits, 
        int numReservationStations, 
        int numPhysicalRegisters,
        int numArchitecturalRegisters,
        int reorderBufferSize,
        bool dynamicBranchPredictor,
        ProcessorMode initialProcessorMode)
    {
        NumFetchUnits = numFetchUnits;
        NumDecodeUnits = numDecodeUnits;
        NumIntegerArithmeticUnits = numIntegerArithmeticUnits;
        NumBranchUnits = numBranchUnits;
        NumLoadStoreUnits = numLoadStoreUnits;
        NumReservationStations = numReservationStations;
        NumPhysicalRegisters = numPhysicalRegisters;
        NumArchitecturalRegisters = numArchitecturalRegisters;
        ReorderBufferSize = reorderBufferSize;
        DynamicBranchPredictor = dynamicBranchPredictor;
        InitialProcessorMode = initialProcessorMode;
    }
}

public enum ProcessorMode
{
    RELEASE,
    DEBUGC,
    DEBUGS
}
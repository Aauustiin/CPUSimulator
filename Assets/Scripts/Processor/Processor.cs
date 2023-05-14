using System;
using System.Collections.Generic;
using System.Linq;

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
            ReservationStations[i] = new ReservationStation(i, this);
        }

        ReorderBuffer = new ReorderBuffer(processorSpecification.ReorderBufferSize, this);

        Registers = new int[processorSpecification.NumPhysicalRegisters];
        RegisterAllocationTable = new RegisterAllocationTable(processorSpecification.NumArchitecturalRegisters, this);

        BranchPredictionUnit = processorSpecification.DynamicBranchPredictor
            ? new DynamicBranchPredictor()
            : new StaticBranchPredictor();

        ProcessorMode = processorSpecification.InitialProcessorMode;
    }

    public void Process(ProgramSpecification programSpecification)
    {
        Instructions = programSpecification.Instructions;
        Memory = programSpecification.InitialMemory;

        while (!_finished)
        {
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
            var fullFetchUnits = _fetchUnits.Where(fetchUnit => fetchUnit.HasOutput());
            var emptyDecodeUnits = _decodeUnits.Where(decodeUnit => decodeUnit.IsFree());
            fullFetchUnits.Zip(emptyDecodeUnits, (fetchUnit, decodeUnit) => decodeUnit.Input = fetchUnit.Pop());
            
            _cycle++;
        }
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
    public int NumFetchUnits;
    public int NumDecodeUnits;
    public int NumIntegerArithmeticUnits;
    public int NumBranchUnits;
    public int NumLoadStoreUnits;
    public int NumReservationStations;
    public int NumPhysicalRegisters;
    public int NumArchitecturalRegisters;
    public int ReorderBufferSize;
    public bool DynamicBranchPredictor;
    public ProcessorMode InitialProcessorMode;

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
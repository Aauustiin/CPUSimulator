using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Processor
{
    private readonly FetchUnit[] _fetchUnits;
    private readonly DecodeUnit[] _decodeUnits;
    private readonly IntegerArithmeticUnit[] _integerArithmeticUnits;
    private readonly BranchUnit[] _branchUnits;
    private readonly LoadStoreUnit[] _loadStoreUnits;

    public readonly IBranchPredictionUnit BranchPredictionUnit;
    public readonly ReservationStation[] ReservationStations;
    public readonly ReorderBuffer ReorderBuffer;

    public readonly int[] Registers;
    public RegisterAllocationTable RegisterAllocationTable;
    
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

        _integerArithmeticUnits = new IntegerArithmeticUnit[processorSpecification.NumIntegerArithmeticUnits];
        for (var i = 0; i < processorSpecification.NumIntegerArithmeticUnits; i++)
        {
            _integerArithmeticUnits[i] = new IntegerArithmeticUnit(this);
        }

        _branchUnits = new BranchUnit[processorSpecification.NumBranchUnits];
        for (var i = 0; i < processorSpecification.NumBranchUnits; i++)
        {
            _branchUnits[i] = new BranchUnit(this);
        }

        _loadStoreUnits = new LoadStoreUnit[processorSpecification.NumLoadStoreUnits];
        for (var i = 0; i < processorSpecification.NumLoadStoreUnits; i++)
        {
            _loadStoreUnits[i] = new LoadStoreUnit(this);
        }

        ReservationStations = new ReservationStation[processorSpecification.NumReservationStations];
        for (var i = 0; i < processorSpecification.NumReservationStations; i++)
        {
            ReservationStations[i] = new ReservationStation(i, this);
        }

        ReorderBuffer = new ReorderBuffer(processorSpecification.ReorderBufferSize);

        Registers = new int[processorSpecification.NumPhysicalRegisters];
        RegisterAllocationTable = new RegisterAllocationTable(processorSpecification.NumArchitecturalRegisters);

        BranchPredictionUnit = new NeverBranchUnit();
    }

    private void Process(ProgramSpecification programSpecification)
    {
        Instructions = programSpecification.Instructions;
        Memory = programSpecification.InitialMemory;
        ProcessorMode = programSpecification.InitialProcessorMode;
        
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

            // Step 2: Assign new data.

            // TODO: Commit results from the reorder buffer to the physical register files?

            // TODO: Move results from execution units to the reorder buffer? 

            // TODO: Issue from reservation stations to free execution units

            // Assign decoded instructions to free reservation stations.
            var fullDecodeUnits = _decodeUnits.Where(decodeUnit => decodeUnit.HasOutput()).ToArray();
            var freeReservationStations = ReservationStations.Where(reservationStation =>
                reservationStation.GetState() == ReservationStationState.FREE).ToArray();
            for (var i = 0; (i < freeReservationStations.Count()) & (i < fullDecodeUnits.Count()) & (!ReorderBuffer.IsFull()); i++)
            {
                ReorderBuffer.Issue(new ReorderBufferEntry());
                freeReservationStations[i].SetReservationStationData(fullDecodeUnits[i].Pop().Value);
            }

            // Assign fetched instructions to free decode units.
            var fullFetchUnits = _fetchUnits.Where(fetchUnit => fetchUnit.HasOutput());
            var emptyDecodeUnits = _decodeUnits.Where(decodeUnit => decodeUnit.IsFree());
            fullFetchUnits.Zip(emptyDecodeUnits, (fetchUnit, decodeUnit) => decodeUnit.Input = fetchUnit.Pop());
            
            _cycle++;
        }
    }

    //public void Halt()
    //{
    //    _finished = true;
    //
    //    Debug.Log("Finished!");
    //    Debug.Log("Registers: " + string.Join(',', Registers));
    //    Debug.Log("Memory: " + string.Join(',', Memory));
    //    Debug.Log("Instructions Executed: " + InstructionsExecuted);
    //    Debug.Log("Cycles Taken: " + _cycle);
    //    Debug.Log("Instructions Per Cycle (IPC): " + ((float)InstructionsExecuted/_cycle).ToString("N2"));
    //}

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
            potentialRegisters.Remove(entry.Register);
        }

        if (potentialRegisters.Count == 0) return null;
        return potentialRegisters[0];
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

    public ProcessorSpecification(
        int numFetchUnits, 
        int numDecodeUnits, 
        int numIntegerArithmeticUnits,
        int numBranchUnits, 
        int numLoadStoreUnits, 
        int numReservationStations, 
        int numPhysicalRegisters,
        int numArchitecturalRegisters,
        int reorderBufferSize)
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
    }
}

public enum ProcessorMode
{
    RELEASE,
    DEBUGC,
    DEBUGS
}
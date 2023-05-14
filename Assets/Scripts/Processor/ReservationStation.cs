using System;
using System.Collections.Generic;
using System.Linq;

public class ReservationStation
{
    private ReservationStationData? _reservationStationData;
    private readonly Processor _processor;
    private ReservationStationState _state;
    private readonly List<int> _subscriptions;

    public ReservationStation(int id, Processor processor)
    {
        _processor = processor;
        _state = ReservationStationState.FREE;
        _subscriptions = new List<int>();
        _processor.BranchMispredict += OnBranchMispredict;
    }

    ~ReservationStation()
    {
        _processor.BranchMispredict -= OnBranchMispredict;
    }
    
    public void SetReservationStationData(ReservationStationData reservationStationData)
    {
        _reservationStationData = reservationStationData;
        
        foreach (var source in reservationStationData.Sources)
        {
            if (source == null) continue;
            _processor.ReorderBuffer.Entries[source.Value].ValueProvided += OnSourceUpdated;
            _subscriptions.Add(source.Value);
        }
        
        UpdateState();
    }

    private void OnSourceUpdated(int source)
    {
        _processor.ReorderBuffer.Entries[source].ValueProvided -= OnSourceUpdated;
        _subscriptions.Remove(source);
        
        var robEntry = _processor.ReorderBuffer.Entries[source];
        var robEntryValue = robEntry.GetValue();

        var sourceIndex = Array.IndexOf(_reservationStationData.Value.Sources, source);
        _reservationStationData.Value.Sources[sourceIndex] = null;
        _reservationStationData.Value.SourceValues[sourceIndex] = robEntryValue;
        
        UpdateState();
    }

    public void Issue()
    {
        var executionUnit = Array.Find(_processor.ExecutionUnits,
            unit => unit.IsFree() & unit.GetCompatibleOpcodes().Contains(_reservationStationData.Value.Opcode));
        executionUnit.SetInput(_reservationStationData.Value);
        Clear();
        
    }
    
    private void UpdateState()
    {
        if (_reservationStationData == null) _state = ReservationStationState.FREE;
        else if (_reservationStationData.Value.SourceValues.All(value => value != null))
            _state = ReservationStationState.READY;
        else _state = ReservationStationState.WAITING;
    }
    
    public ReservationStationState GetState()
    {
        return _state;
    }

    private void OnBranchMispredict(int fetchNum)
    {
        if (!((_reservationStationData != null) & (_reservationStationData.Value.FetchNum > fetchNum))) return;

        Clear();
    }

    private void Clear()
    {
        foreach (var entry in _subscriptions)
        {
            _processor.ReorderBuffer.Entries[entry].ValueProvided -= OnSourceUpdated;
        }
        _subscriptions.Clear();
        _reservationStationData = null;
        _state = ReservationStationState.FREE;
    }
}

public struct ReservationStationData
{
    public Opcode Opcode;
    public int? DestinationRegister; // Do we need this? Reorder buffer keeps track of this info right?
    public int?[] Sources;
    public int?[] SourceValues;
    public int FetchNum;

    public ReservationStationData(Opcode opcode, int? destinationRegister, int?[] sources, int?[] sourceValues, int fetchNum)
    {
        Opcode = opcode;
        DestinationRegister = destinationRegister;
        Sources = sources;
        SourceValues = sourceValues;
        FetchNum = fetchNum;
    }
}

public enum ReservationStationState
{
    FREE, // Currently has no data in it.
    WAITING, // Has data, but it waiting for source values to be provided.
    READY // Source values are provided, is waiting to issue to an execution unit.
}
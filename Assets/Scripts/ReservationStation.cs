using System;
using System.Linq;

public class ReservationStation
{
    public readonly int Id;
    public ReservationStationData? ReservationStationData;
    private readonly Processor _processor;
    private ReservationStationState _state;

    public ReservationStation(int id, Processor processor)
    {
        Id = id;
        _processor = processor;
        _state = ReservationStationState.FREE;
    }

    public void SetReservationStationData(ReservationStationData reservationStationData)
    {
        ReservationStationData = reservationStationData;
        if (reservationStationData.DestinationRegister != null)
            _processor.Scoreboard[reservationStationData.DestinationRegister.Value] = Id;
        foreach (var source in reservationStationData.Sources)
        {
            if (source != null)
            {
                _processor.ReservationStations[source.Value].ResultGenerated += OnSourceUpdated;
            }
        }
        UpdateState();
    }

    private void OnSourceUpdated(Result result)
    {
        var sourceIndex = Array.IndexOf(ReservationStationData.Value.Sources, result.ReservationStationId);
        ReservationStationData.Value.Sources[sourceIndex] = null;
        ReservationStationData.Value.SourceValues[sourceIndex] = result.Value;
        UpdateState();
    }

    private void UpdateState()
    {
        if (ReservationStationData == null) _state = ReservationStationState.FREE;
        else if (ReservationStationData.Value.SourceValues.All(value => value != null))
            _state = ReservationStationState.READY;
        else _state = ReservationStationState.WAITING;
    }
    
    public ReservationStationState GetState()
    {
        return _state;
    }

    public event System.Action<Result> ResultGenerated;

    public void TriggerResultGenerated(Result result)
    {
        ResultGenerated?.Invoke(result);
        ReservationStationData = null;
        _state = ReservationStationState.FREE;
    }
}

public struct ReservationStationData
{
    public Opcode Opcode;
    public int? DestinationRegister;
    public int?[] Sources;
    public int?[] SourceValues;

    public ReservationStationData(Opcode opcode, int? destinationRegister, int?[] sources, int?[] sourceValues)
    {
        Opcode = opcode;
        DestinationRegister = destinationRegister;
        Sources = sources;
        SourceValues = sourceValues;
    }
}

public enum ReservationStationState
{
    FREE, // Currently has no data in it.
    WAITING, // Has data, but it waiting for source values to be provided.
    READY // Source values are provided, is waiting to issue to an execution unit.
}
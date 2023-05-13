using System;
using System.Collections.Generic;
using System.Linq;

public class DecodeUnit
{
    public (Instruction, int)? Input;
    private readonly Processor _processor;

    public DecodeUnit(Processor processor)
    {
        Input = null;
        _processor = processor;
        _processor.BranchMispredict += OnBranchMispredict;
    }

    ~DecodeUnit()
    {
        _processor.BranchMispredict -= OnBranchMispredict;
    }

    public void Decode()
    {
        // If we haven't been given anything to decode, or we haven't been able to give our output to a reservation station, do nothing.
        if (Input == null) return;
        
        var instruction = Input.Value.Item1;
        var opcode = instruction.Opcode;
        var convertedInstruction = _processor.RegisterAllocationTable.ConvertInstruction(instruction);
        
        // If there is no available destination register, then stall.
        if (convertedInstruction == null) return;

        // If there is no space in the ROB or the reservation stations, stall.
        var reservationStation = _processor.GetAvailableReservationStation();
        if ((reservationStation == null) | _processor.ReorderBuffer.IsFull()) return;
        
        // Make a ROB entry.
        // Make a reservation station entry.
        var sources = new List<int?>();
        var sourceValues = new List<int?>();
        foreach (var originalSource in instruction.Sources)
        {
            var sourceInformation = GetSourceInformation(originalSource);
            sources.Add(sourceInformation.Item1);
            sourceValues.Add(sourceInformation.Item2);
        }
        var reservationStationData = new ReservationStationData(instruction.Opcode, instruction.Destination, sources.ToArray(),
            sourceValues.ToArray(), Input.Value.Item2);
        _processor.ReservationStations[reservationStation.Value].SetReservationStationData(reservationStationData);
        
        Input = null;
    }

    private (int?, int?) GetSourceInformation(int originalSource)
    {
        var robEntryIndex = Array.FindIndex(_processor.ReorderBuffer.Entries, entry => entry.Register == originalSource);
        
        // If there's no ROB entry for this, then just use whatever is in the physical register.
        if (robEntryIndex == -1) return (null, _processor.Registers[originalSource]);

        // Otherwise, return whatever is indicated by the ROB entry;
        var robEntryValue = _processor.ReorderBuffer.Entries[robEntryIndex].GetValue();
        return robEntryValue == null ? (robEntryIndex, null) : (null, robEntryValue);
    }
    
    public bool IsFree()
    {
        return Input == null;
    }

    private void OnBranchMispredict(int fetchNum)
    {
        if (Input == null) return;

        if (Input.Value.Item2 > fetchNum) Input = null;
    }
}
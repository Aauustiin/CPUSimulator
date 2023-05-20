using System;
using System.Collections.Generic;
using System.Linq;

public class DecodeUnit
{
    public FetchData? Input;
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
        
        var instruction = Input.Value.Instruction;
        var convertedInstruction = _processor.RegisterAllocationTable.ConvertInstruction(instruction);
        
        // If there is no available destination register, then stall.
        if (convertedInstruction == null) return;

        // If there is no space in the ROB or the reservation stations, stall.
        var reservationStation = _processor.GetAvailableReservationStation();
        if ((reservationStation == null) | _processor.ReorderBuffer.IsFull()) return;
        
        // Make a ROB entry.
        _processor.ReorderBuffer.Issue(instruction.Opcode, Input.Value.FetchNum, convertedInstruction.Value.Destination, GetResultValue(convertedInstruction.Value));
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
            sourceValues.ToArray(), Input.Value.FetchNum, Input.Value.Prediction, Input.Value.ProgramCounter);

        // LOAD / STORE STUFF

        var knownSource = false;

        if (instruction.Opcode == Opcode.LOADI) knownSource = true;
        else if (instruction.Opcode == Opcode.LOAD) knownSource = reservationStationData.SourceValues[0] != null;

        if (knownSource)
        {
            if (_processor.ReorderBuffer.Entries.Any(entry => entry.Opcode == Opcode.STORE))
            {
                
            }
        }
        
        // END OF LOAD / STORE STUFF
        
        _processor.ReservationStations[reservationStation.Value].SetReservationStationData(reservationStationData);
        
        Input = null;
    }

    private (int?, int?) GetSourceInformation(int originalSource)
    {
        var robEntryIndex = Array.FindIndex(_processor.ReorderBuffer.Entries, entry => entry.GetDestination() == originalSource);
        
        // If there's no ROB entry for this, then just use whatever is in the physical register.
        if (robEntryIndex == -1) return (null, _processor.Registers[originalSource]);

        // Otherwise, return whatever is indicated by the ROB entry;
        var robEntryValue = _processor.ReorderBuffer.Entries[robEntryIndex].GetValue();
        return robEntryValue == null ? (robEntryIndex, null) : (null, robEntryValue);
    }

    private int? GetResultValue(Instruction instruction)
    {
        return instruction.Opcode switch
        {
            Opcode.ADD => null,
            Opcode.ADDI => null,
            Opcode.SUB => null,
            Opcode.SUBI => null,
            Opcode.MUL => null,
            Opcode.DIV => null,
            Opcode.MOD => null,
            Opcode.COPY => _processor.ReorderBuffer.GetRegisterValue(instruction.Sources[0]),
            Opcode.COPYI => instruction.Sources[0],
            Opcode.LOAD => null,
            Opcode.LOADI => null,
            Opcode.STORE => null,
            Opcode.BRANCHE => null,
            Opcode.BRANCHG => null,
            Opcode.BRANCHGE => null,
            Opcode.JUMP => null,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public bool IsFree()
    {
        return Input == null;
    }

    private void OnBranchMispredict(int fetchNum)
    {
        if (Input == null) return;

        if (Input.Value.FetchNum > fetchNum) Input = null;
    }
}
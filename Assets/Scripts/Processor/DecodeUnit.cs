using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        var convertedInstruction = _processor.RegisterRenaming ? _processor.RegisterAllocationTable.ConvertInstruction(instruction) : instruction;
        
        // If there is no available destination register, then stall.
        if (convertedInstruction == null) return;

        // If there is no space in the ROB or the reservation stations, stall. - ISSUE 2: But what about for instructions that don't need a reservation station?
        var reservationStation = _processor.GetAvailableReservationStation();
        if ((reservationStation == null) | _processor.ReorderBuffer.IsFull()) return;

        // Make a ROB entry.
        _processor.ReorderBuffer.Issue(instruction.Opcode, Input.Value.FetchNum, convertedInstruction.Value.Destination, GetResultValue(convertedInstruction.Value));
        // Make a reservation station entry. - ISSUE 1: Why have I assumed that every instruction needs a reservation station entry?
        var sourceInfo = GetSourceInformation(instruction, Input.Value.FetchNum);
        var reservationStationData = new ReservationStationData(instruction.Opcode, instruction.Destination, sourceInfo.Item1.ToArray(),
            sourceInfo.Item2.ToArray(), Input.Value.FetchNum, Input.Value.Prediction, Input.Value.ProgramCounter);

        _processor.ReservationStations[reservationStation.Value].SetReservationStationData(reservationStationData);
        
        Input = null;
    }

    public static readonly Opcode[] AllRegOpcodes =
    {
        Opcode.ADD,
        Opcode.SUB,
        Opcode.MUL,
        Opcode.DIV,
        Opcode.MOD,
        Opcode.COPY,
        Opcode.STORE,
        Opcode.LOAD
    };
    
    public static readonly Opcode[] RegImmOpcodes =
    {
        Opcode.ADDI,
        Opcode.SUBI
    };
    
    private (int?[], int?[]) GetSourceInformation(Instruction instruction, int fetchNum)
    {
        if (BranchUnit.CompatibleOpcodes.Contains(instruction.Opcode))
        {
            var sourceA = GetRegisterSourceInfo(instruction.Sources[0], fetchNum);
            var sourceB = GetRegisterSourceInfo(instruction.Sources[1], fetchNum);
            return (new int?[] { sourceA.Item1, sourceB.Item1 }, new int?[] { sourceA.Item2, sourceB.Item2 });
        }
        if (AllRegOpcodes.Contains(instruction.Opcode))
        {
            var info = instruction.Sources.Select(source => GetRegisterSourceInfo(source, fetchNum));
            var sources = info.Select(item => item.Item1).ToArray();
            var sourceValues = info.Select(item => item.Item2).ToArray();
            return (sources, sourceValues);
        }
        if (RegImmOpcodes.Contains(instruction.Opcode))
        {
            var sourceA = GetRegisterSourceInfo(instruction.Sources[0], fetchNum);
            var sourceB = GetImmediateSourceInfo(instruction.Sources[1]);
            return (new int?[] { sourceA.Item1, sourceB.Item1 }, new int?[] { sourceA.Item2, sourceB.Item2 });
        }
        if (instruction.Opcode == Opcode.COPYI)
        {
            var sourceInfo = GetImmediateSourceInfo(instruction.Sources[0]);
            return (new int?[] { sourceInfo.Item1 }, new int?[] { sourceInfo.Item2 });
        }
        if (instruction.Opcode == Opcode.LOADI) 
        {
            return (new int?[] { null }, new int?[] { instruction.Sources[0] });
        }

        return (new int?[] { }, new int?[] { });
    }

    private (int?, int?) GetImmediateSourceInfo(int originalSource)
    {
        return (null, originalSource);
    }
    
    private (int?, int?) GetRegisterSourceInfo(int originalSource, int fetchNum)
    {
        var robEntries = Array.FindAll(_processor.ReorderBuffer.Entries, entry => (entry.Opcode != Opcode.STORE) & (entry.GetDestination() == originalSource) & (entry.FetchNum < fetchNum));
        var index = robEntries.Length == 0 ? -1 : Array.FindIndex(robEntries, entry => entry.FetchNum == robEntries.Max(x => x.FetchNum));

        // If there's no ROB entry for this, then just use whatever is in the physical register.
        if (index == -1) return (null, _processor.Registers[originalSource]);

        // Otherwise, return whatever is indicated by the ROB entry;
        var robEntryValue = robEntries[index].GetValue();
        var robEntryIndex = Array.FindIndex(_processor.ReorderBuffer.Entries,
            entry => entry.FetchNum == robEntries[index].FetchNum);
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

    public override string ToString()
    {
        return Input.ToString();
    }
}

public class DecodeUnitComparer : IComparer<DecodeUnit>
{
    public int Compare(DecodeUnit x, DecodeUnit y)
    {
        if (x.Input.Value.FetchNum > y.Input.Value.FetchNum) return 1;
        if (x.Input.Value.FetchNum < y.Input.Value.FetchNum) return -1;
        return 0;
    }
}
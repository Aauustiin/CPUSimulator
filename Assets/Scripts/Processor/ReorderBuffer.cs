using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReorderBuffer
{
    public ReorderBufferEntry[] Entries;
    private int? _issuePointer, _commitPointer;
    private readonly Processor _processor;
    
    public ReorderBuffer(int size, Processor processor)
    {
        Entries = new ReorderBufferEntry[size];
        for (var i = 0; i < size; i++)
        {
            Entries[i] = new ReorderBufferEntry(i);
        }

        _commitPointer = null;
        _issuePointer = 0;
        
        _processor = processor;
        _processor.BranchMispredict += OnBranchMispredict;
    }

    ~ReorderBuffer()
    {
        _processor.BranchMispredict -= OnBranchMispredict;
    }

    // This is for adding something to the Reorder Buffer
    public void Issue(Opcode opcode, int fetchNum, int? destination, int? value)
    {
        if (IsFull()) return; // Can't issue anything if we don't have space.

        if (_commitPointer == null) _commitPointer = _issuePointer;
        Entries[_issuePointer.Value].Initialise(opcode, fetchNum, destination, value);
        
        _issuePointer = IsFull() ? null : (_issuePointer + 1) % Entries.Length;
    }
    
    // Update a given entry with a new value and/or destination.
    public void Update(int fetchNum, int? value, int? destination)
    {
        var entryNum = Array.FindIndex(Entries, entry => entry.FetchNum == fetchNum);

        // Update Value
        if (value != null) Entries[entryNum].SetValue(value.Value);
        if (destination != null) Entries[entryNum].SetDestination(destination.Value);
        
        // Commit anything that needs to be committed.
        if (_commitPointer == null) throw new Exception();
        while ((Entries[_commitPointer.Value].GetValue() != null) & (Entries[_commitPointer.Value].FetchNum <= fetchNum) & ((Entries[_commitPointer.Value].GetDestination() != null) | BranchUnit.CompatibleOpcodes.Contains(Entries[_commitPointer.Value].Opcode)))
        {
            if (_issuePointer == null) _issuePointer = _commitPointer;
            
            if (Entries[_commitPointer.Value].Opcode == Opcode.STORE)
                _processor.Memory[Entries[_commitPointer.Value].GetDestination().Value] = Entries[_commitPointer.Value].GetValue().Value;
            else if (!BranchUnit.CompatibleOpcodes.Contains(Entries[_commitPointer.Value].Opcode))
                _processor.Registers[Entries[_commitPointer.Value].GetDestination().Value] = Entries[_commitPointer.Value].GetValue().Value;
            
            Entries[_commitPointer.Value].Free = true;
            _commitPointer = (_commitPointer + 1) % Entries.Length;
        }
        if (_commitPointer == _issuePointer) _commitPointer = null;
    }

    public int? GetRegisterValue(int register)
    {
        var index = Array.FindIndex(Entries, entry => (entry.GetDestination() == register) & (entry.Opcode != Opcode.STORE));
        return index == -1 ? _processor.Registers[register] : Entries[index].GetValue();
    }
    
    public bool IsFull()
    {
        return Entries.All(entry => entry.Free == false);
    }

    public bool IsEmpty()
    {
        return Entries.All(entry => entry.Free);
    }

    private void OnBranchMispredict(int fetchNum)
    {
        if (_issuePointer == null) _issuePointer = _commitPointer;
        _issuePointer = (_issuePointer - 1 + Entries.Length) % Entries.Length;

        while (Entries[_issuePointer.Value].FetchNum != fetchNum)
        {
            Entries[_issuePointer.Value].Free = true;
            _issuePointer = (_issuePointer - 1 + Entries.Length) % Entries.Length;
        }
        Entries[_issuePointer.Value].Free = true;

        if (_issuePointer == _commitPointer) _commitPointer = null;
    }

    public override string ToString()
    {
        return "Issue: " + _issuePointer + ", Commit: " + _commitPointer + "\n" + String.Join('\n', Entries.Select(entry => entry.ToString()));
    }
}

public class ReorderBufferEntry
{
    public int Id;
    public bool Free;
    
    public Opcode Opcode;
    public int FetchNum;
    
    private int? _destination;
    private int? _value;

    public event System.Action<int> ValueProvided;

    public ReorderBufferEntry(int id)
    {
        Id = id;
        Free = true;
    }
    
    public void Initialise(Opcode  opcode, int fetchNum, int? destination, int? value)
    {
        Free = false;
        
        Opcode = opcode;
        FetchNum = fetchNum;
        _destination = destination;
        _value = value;
    }
    
    public void SetValue(int value)
    {
        _value = value;
        ValueProvided?.Invoke(Id);
    }

    public int? GetValue()
    {
        return _value;
    }

    public void SetDestination(int destination)
    {
        _destination = destination;
    }

    public int? GetDestination()
    {
        return Free ? null : _destination;
    }

    public override string ToString()
    {
        if (!Free)
            return "ID: " + Id + ", Opcode: " + Opcode + ", Destination: " + _destination +
                   ", Value: " + _value + ", Fetch Number: " + FetchNum;
        
        return "ID: " + Id + ", Free";
    }
}

public class RobEntryComparer : IComparer<ReorderBufferEntry>
{
    public int Compare(ReorderBufferEntry x, ReorderBufferEntry y)
    {
        if (x.FetchNum < y.FetchNum) return -1;
        if (x.FetchNum == y.FetchNum) return 0;
        return 1;
    }
}
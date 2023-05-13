using System;
using System.Linq;

public class ReorderBuffer
{
    public readonly ReorderBufferEntry[] Entries;
    private int _issuePointer, _commitPointer = -1;
    private Processor _processor;
    
    public ReorderBuffer(int size)
    {
        Entries = new ReorderBufferEntry[size];
        for (var i = 0; i < size; i++)
        {
            Entries[i].Id = i;
        }
    }

    // This is for adding something to the Reorder Buffer
    public void Issue(ReorderBufferEntry entry)
    {
        if (IsFull()) return; // Can't issue anything if we don't have space.
        Entries[_issuePointer] = entry;
        _issuePointer = (_issuePointer + 1) % Entries.Length;
    }
    
    // Update a given entry with a new value and/or destination.
    public void Update(int entryNum, int? value, int? destination)
    {
        // Update Value
        if (value != null) Entries[entryNum].SetValue(value.Value);
        if (destination != null) Entries[entryNum].SetDestination(destination.Value);
        // Commit anything that needs to be committed.
        while ((Entries[_commitPointer].GetValue() != null) & (Entries[_commitPointer].GetDestination() != null))
        {
            if (Entries[_commitPointer].Opcode != Opcode.STORE)
                _processor.Registers[Entries[_commitPointer].GetDestination().Value] = Entries[_commitPointer].GetValue().Value;
            else _processor.Memory[Entries[_commitPointer].GetDestination().Value] = Entries[_commitPointer].GetValue().Value;
            Entries[_commitPointer] = null;
            _commitPointer = (_commitPointer + 1) % Entries.Length;
        }
    }
    
    public bool IsFull()
    {
        return Entries.All(entry => entry != null);
    }
}

public class ReorderBufferEntry
{
    private int? _destination;
    private int? _value;
    public int Id;
    public Opcode Opcode;

    public event System.Action<int> ValueProvided;
    
    public event System.Action<int> DestinationProvided;

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
        DestinationProvided?.Invoke(Id);
    }

    public int? GetDestination()
    {
        return _destination;
    }
    
    public ReorderBufferEntry(int destination, int value, Opcode opcode, int id)
    {
        _destination = destination;
        _value = null;
        Id = id;
    }
}
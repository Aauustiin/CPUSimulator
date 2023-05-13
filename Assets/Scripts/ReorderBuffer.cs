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

    public void Issue(ReorderBufferEntry entry)
    {
        if (IsFull()) return; // Can't issue anything if we don't have space.
        Entries[_issuePointer] = entry;
        _issuePointer = (_issuePointer + 1) % Entries.Length;
    }
    
    public void Update(int entryNum, int value)
    {
        // Update Value
        Entries[entryNum].SetValue(value);
        // Commit anything that needs to be committed.
        while ((Entries[_commitPointer].GetValue() != null) & (Entries[_commitPointer].GetDestination() != null))
        {
            _processor.Registers[Entries[_commitPointer].GetDestination().Value] = value;
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
    
    public ReorderBufferEntry(int destination, int id)
    {
        _destination = destination;
        _value = null;
        Id = id;
    }
}
using System.Linq;

public class ReorderBuffer
{
    public ReorderBufferEntry[] Entries;
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
        while (Entries[_commitPointer].GetValue() != null)
        {
            _processor.Registers[Entries[_commitPointer].Register] = value;
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
    public int Register;
    private int? _value;
    public int Id;

    public event System.Action<int> ValueProvided;

    public void SetValue(int value)
    {
        _value = value;
        ValueProvided?.Invoke(Id);
    }

    public int? GetValue()
    {
        return _value;
    }
    
    public ReorderBufferEntry(int register, int id)
    {
        Register = register;
        _value = null;
        Id = id;
    }
}
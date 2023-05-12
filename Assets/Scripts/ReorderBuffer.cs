using System.Linq;

public class ReorderBuffer
{
    private ReorderBufferEntry[] _entries;
    private int _issuePointer, _commitPointer = -1;
    private Processor _processor;
    
    public ReorderBuffer(int size)
    {
        _entries = new ReorderBufferEntry[size];
    }

    public void Issue(ReorderBufferEntry entry)
    {
        if (IsFull()) return; // Can't issue anything if we don't have space.
        _entries[_issuePointer] = entry;
        _issuePointer = (_issuePointer + 1) % _entries.Length;
    }
    
    public void Update(int entryNum, int value)
    {
        // Update Value
        _entries[entryNum].SetValue(value);
        // Commit anything that needs to be committed.
        while (_entries[_commitPointer].GetValue() != null)
        {
            _processor.Registers[_entries[_commitPointer].Register] = value;
            _entries[_commitPointer] = null;
            _commitPointer = (_commitPointer + 1) % _entries.Length;
        }
    }
    
    public bool IsFull()
    {
        return _entries.All(entry => entry != null);
    }
}

public class ReorderBufferEntry
{
    public int Register;
    private int? _value;

    public event System.Action ValueProvided;

    public void SetValue(int value)
    {
        _value = value;
        ValueProvided?.Invoke();
    }

    public int? GetValue()
    {
        return _value;
    }
    
    public ReorderBufferEntry(int register)
    {
        Register = register;
        _value = null;
    }
}
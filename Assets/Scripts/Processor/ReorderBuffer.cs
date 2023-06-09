using System;
using System.Linq;

public class ReorderBuffer
{
    public ReorderBufferEntry[] Entries;
    private int _issuePointer, _commitPointer;
    private readonly Processor _processor;
    
    public ReorderBuffer(int size, Processor processor)
    {
        Entries = new ReorderBufferEntry[size];
        for (var i = 0; i < size; i++)
        {
            Entries[i] = new ReorderBufferEntry(i);
        }

        _commitPointer = -1;
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
        if (_commitPointer == -1) _commitPointer = _issuePointer;
        Entries[_issuePointer].Initialise(opcode, fetchNum, destination, value);
        _issuePointer = (_issuePointer + 1) % Entries.Length;
    }
    
    // Update a given entry with a new value and/or destination.
    public void Update(int fetchNum, int? value, int? destination)
    {
        var entryNum = Array.FindIndex(Entries, entry => entry.FetchNum == fetchNum);
        // Update Value
        if (value != null) Entries[entryNum].SetValue(value.Value);
        if (destination != null) Entries[entryNum].SetDestination(destination.Value);
        // Commit anything that needs to be committed.
        while ((Entries[_commitPointer].GetValue() != null) & (Entries[_commitPointer].GetDestination() != null))
        {
            if (Entries[_commitPointer].Opcode != Opcode.STORE)
                _processor.Registers[Entries[_commitPointer].GetDestination().Value] = Entries[_commitPointer].GetValue().Value;
            else if (Entries[_commitPointer].GetDestination() != null) 
                _processor.Memory[Entries[_commitPointer].GetDestination().Value] = Entries[_commitPointer].GetValue().Value;
            Entries[_commitPointer].Free = true;
            _commitPointer = (_commitPointer + 1) % Entries.Length;
        }
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
        var remainingEntries = Entries.Where(entry => entry.FetchNum < fetchNum).ToArray();
        for (var i = 0; i < Entries.Length; i++)
        {
            Entries[i] = i < remainingEntries.Length ? remainingEntries[i] : new ReorderBufferEntry(i);
        }

        _commitPointer = remainingEntries.Length - 1;
        _issuePointer = Entries.Length - 1;
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
        return _destination;
    }

    public override string ToString()
    {
        if (!Free)
            return "ID: " + Id + ", Opcode: " + Opcode + ", Destination: " + _destination +
                   ", Value: " + _value + ", Fetch Number: " + FetchNum;
        else return "ID: " + Id + ", Free";
    }
}
using System;

public interface IExecutionUnit
{
    public Tuple<Opcode, int, int> Execute();
}

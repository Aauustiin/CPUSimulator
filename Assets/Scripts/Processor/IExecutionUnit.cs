using System.Collections.Generic;

public interface IExecutionUnit
{
    public void Execute();

    public IEnumerable<Opcode> GetCompatibleOpcodes();

    public bool IsFree();

    public void SetInput(ReservationStationData data);
}

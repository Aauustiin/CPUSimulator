public interface IExecutionUnit
{
    public void Execute();

    public Opcode[] GetCompatibleOpcodes();

    public bool IsFree();

    public void SetInput(ReservationStationData data);
}

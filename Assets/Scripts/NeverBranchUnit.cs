public class NeverBranchUnit : IBranchPredictionUnit
{
    public bool Predict(Instruction instruction)
    {
        return false;
    }

    public void Notify(bool taken)
    {
        // Do nothing.
    }
}

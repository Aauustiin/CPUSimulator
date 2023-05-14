public class StaticBranchPredictor : IBranchPredictionUnit
{
    public bool Predict(Instruction instruction)
    {
        return true;
    }

    public void Notify(bool prediction, bool taken)
    {
        // Do nothing
    }
}

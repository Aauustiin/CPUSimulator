public interface IBranchPredictionUnit
{
    // Asks the branch predictor whether the given branch should be taken or not.
    public bool Predict(Instruction instruction);
    // Notifies the branch predictor whether or not a branch was actually taken.
    public void Notify(bool taken);
}

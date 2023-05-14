using System;

public class DynamicBranchPredictor : IBranchPredictionUnit
{
    private bool? _branchA; 
    private bool? _branchB;

    private PredictorState? _state;
    
    public bool Predict(Instruction instruction)
    {
        if (_state == null) return true;
        return (!((_state == PredictorState.WEAK_NO) | (_state == PredictorState.STRONG_NO)));
    }
    
    public void Notify(bool prediction, bool taken)
    {
        if (_state == null)
        {
            if (_branchA == null) _branchA = taken;
            else
            {
                _branchB = _branchA;
                _branchA = taken;
                
                // Initialise state
                if (_branchA.Value & _branchB.Value) _state = PredictorState.STRONG_YES;
                else if (!_branchA.Value & _branchB.Value) _state = PredictorState.WEAK_YES;
                else if (_branchA.Value & !_branchB.Value) _state = PredictorState.WEAK_NO;
                else _state = PredictorState.STRONG_NO;
            }
        }
        else
        {
            if (!prediction & taken) _state = IncrementPredictorState(_state.Value);
            else if (prediction & !taken) _state = DecrementPredictorState(_state.Value);
        }
    }
    
    private PredictorState IncrementPredictorState(PredictorState state)
    {
        return state switch
        {
            PredictorState.STRONG_NO => PredictorState.WEAK_NO,
            PredictorState.WEAK_NO => PredictorState.WEAK_YES,
            PredictorState.WEAK_YES => PredictorState.STRONG_YES,
            PredictorState.STRONG_YES => PredictorState.STRONG_YES,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private PredictorState DecrementPredictorState(PredictorState state)
    {
        return state switch
        {
            PredictorState.STRONG_NO => PredictorState.STRONG_NO,
            PredictorState.WEAK_NO => PredictorState.STRONG_NO,
            PredictorState.WEAK_YES => PredictorState.WEAK_NO,
            PredictorState.STRONG_YES => PredictorState.WEAK_YES,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public enum PredictorState
{
    STRONG_NO,
    WEAK_NO,
    WEAK_YES,
    STRONG_YES
}


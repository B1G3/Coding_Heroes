using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class IfState : IUnitState
{
    private readonly System.Func<Unit, bool> _condition;
    private readonly List<IUnitState> _trueStates;
    private readonly List<IUnitState> _falseStates;
    private bool _isCompleted = false;

    public IfState(System.Func<Unit, bool> condition, List<IUnitState> trueStates, List<IUnitState> falseStates = null)
    {
        _condition = condition;
        _trueStates = trueStates ?? new List<IUnitState>();
        _falseStates = falseStates ?? new List<IUnitState>();
    }

    public void Enter(Unit unit)
    {
        _isCompleted = false;
        ExecuteCondition(unit).Forget();
    }

    private async UniTaskVoid ExecuteCondition(Unit unit)
    {
        var statesToExecute = _condition(unit) ? _trueStates : _falseStates;
        
        foreach (var state in statesToExecute)
        {
            state.Enter(unit);
            await UniTask.WaitUntil(() => state.IsCompleted(unit));
            state.Exit(unit);
        }
        
        _isCompleted = true;
    }

    public void Update(Unit unit) { }

    public bool IsCompleted(Unit unit)
    {
        return _isCompleted;
    }

    public void Exit(Unit unit) { }
}
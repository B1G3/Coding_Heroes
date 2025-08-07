using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class WhileState : IUnitState
{
    private readonly System.Func<Unit, bool> _condition;
    private readonly List<IUnitState> _loopStates;
    private bool _isCompleted = false;

    public WhileState(System.Func<Unit, bool> condition, List<IUnitState> loopStates)
    {
        _condition = condition;
        _loopStates = new List<IUnitState>(loopStates);
    }

    public void Enter(Unit unit)
    {
        _isCompleted = false;
        ExecuteLoop(unit).Forget();
    }

    private async UniTaskVoid ExecuteLoop(Unit unit)
    {
        while (_condition(unit))
        {
            // 루프 내부 상태들을 순차 실행
            foreach (var state in _loopStates)
            {
                state.Enter(unit);
                await UniTask.WaitUntil(() => state.IsCompleted(unit));
                state.Exit(unit);
            }
            
            await UniTask.Yield(); // 한 프레임 대기
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
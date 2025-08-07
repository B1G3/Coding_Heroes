using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WhileState : IUnitState
{
    private readonly List<IUnitState> _loopStates;
    private bool _isCompleted = false;

    public WhileState(List<IUnitState> loopStates)
    {
        _loopStates = new List<IUnitState>(loopStates);
    }

    public void Enter(Unit unit)
    {
        _isCompleted = false;
        WaitForTarget(unit).Forget();
    }

    private async UniTaskVoid WaitForTarget(Unit unit)
    {
        ITarget detectedTarget = null;
    
        // DataContainer가 감지될 때까지 대기
        while (detectedTarget == null)
        {
            Collider[] containers = Physics.OverlapSphere(unit.transform.position, 0.1f);
            foreach (var container in containers)
            {
                if (container.TryGetComponent<DataContainer>(out var dataContainer))
                {
                    dataContainer.OnStop();
                    detectedTarget = dataContainer;
                    break;
                }
            }
        
            await UniTask.Yield();
        }
    
        // 타겟을 발견했으면, 기존 command의 State들 타겟 업데이트
        UpdateExistingStates(unit, detectedTarget);
    
        _isCompleted = true;
    }

    
    private void UpdateExistingStates(Unit unit, ITarget target)
    {
        var unitCommands = unit.GetRemainingCommand(); 
    
        // null 체크 추가!
        if (unitCommands == null || unitCommands.Count == 0)
        {
            Debug.LogWarning("Unit의 command가 아직 설정되지 않음");
            return;
        }
    
        foreach (var state in unitCommands)
        {
            if (state is MoveState moveState)
            {
                moveState.SetTarget(target);
            }
            else if (state is AttackState attackState)
            {
                attackState.SetTarget(target as IAttackable);
            }
        }
    }

    public void Update(Unit unit) { }

    public bool IsCompleted(Unit unit)
    {
        return _isCompleted;
    }

    public void Exit(Unit unit) { }
}
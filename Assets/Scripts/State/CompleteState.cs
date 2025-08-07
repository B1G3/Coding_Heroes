using UnityEngine;

/// <summary>
/// 이미 완료된 상태를 나타내는 더미 State
/// WhileState가 Move/Attack을 대신 처리할 때 사용
/// </summary>
public class CompletedState : IUnitState
{
    private readonly string _originalStateName;
    
    public CompletedState(string originalStateName = "Unknown")
    {
        _originalStateName = originalStateName;
    }

    public void Enter(Unit unit)
    {
        Debug.Log($"CompletedState: {_originalStateName} 즉시 완료 처리");
    }

    public void Update(Unit unit)
    {
        // 아무것도 하지 않음 - 즉시 완료
    }

    public bool IsCompleted(Unit unit)
    {
        return true; // 🎯 항상 완료된 상태
    }

    public void Exit(Unit unit)
    {
        Debug.Log($"CompletedState: {_originalStateName} Exit");
    }

    public override string ToString()
    {
        return $"Completed({_originalStateName})";
    }
}
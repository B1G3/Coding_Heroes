using System;
using UnityEngine;
using static BlockConfig;

public class RailModule : MonoBehaviour, IInput, IOutput
{
    [SerializeField] private Direction inputDirection;
    [SerializeField] private Direction outputDirection;
    
    private IInput connectedInput;
    
    public event Action<Direction, Direction> OnDirectionChanged;
    
    public void SetInputDirection(Direction dir)
    {
        inputDirection = dir;
        OnDirectionChanged?.Invoke(inputDirection, outputDirection);
    }

    public void SetOutputDirection(Direction dir)
    {
        outputDirection = dir;
        OnDirectionChanged?.Invoke(inputDirection, outputDirection);
    }
    
    public void RegisterOutputTarget(IInput target, Direction toDir)
    {
        if (toDir == outputDirection)
        {
            connectedInput = target;
            Debug.Log($"[RailModule] Registered target at {toDir}: {target}");
        }
    }

    public bool CanReceive(Direction fromDir) => fromDir == inputDirection;
    public bool TryReceive(int amount, Direction fromDir)
    {
        if (!CanReceive(fromDir)) return false;
        return true;
    }

    public bool CanSend(Direction toDir) => toDir == outputDirection;
    public bool TrySend(out int amount, Direction toDir)
    {
        amount = 1;
        return toDir == outputDirection;
    }
    
    public void Tick(Vector3Int myPos)
    {
        // 자원 준비
        if (!TrySend(out int amount, outputDirection)) return;

        // 출력 방향으로 다음 박스 찾기
        var target = FlowManager.Instance.FindBoxInDirection(myPos, outputDirection);
        if (target is IInput input && input.CanReceive(outputDirection))
        {
            input.TryReceive(amount, outputDirection);
            Debug.Log($"[Rail] Sent {amount} to {target} at {target.GridPosition}");
        }
    }
    
    public Direction GetInputDirection() => inputDirection;
    public Direction GetOutputDirection() => outputDirection;
}
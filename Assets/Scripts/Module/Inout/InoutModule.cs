using System;
using UnityEngine;
using static BlockConfig;

public abstract class InoutModule : MonoBehaviour, IInput, IOutput
{
    [Header("Flow Directions")]
    [SerializeField] private Direction inputDirection  = Direction.None;
    [SerializeField] private Direction outputDirection = Direction.None;

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

    public Direction GetInputDirection()  => inputDirection;
    public Direction GetOutputDirection() => outputDirection;
    
    public virtual bool CanReceive(Direction dir)   => dir == inputDirection;
    public virtual bool TryReceive(int amt, Direction dir)
    {
        return CanReceive(dir);
    }
    
    public virtual bool CanSend(Direction dir)       => dir == outputDirection;
    public virtual bool TrySend(out int amt, Direction dir)
    {
        amt = 1;
        return CanSend(dir);
    }
}
using System;
using UnityEngine;
using static BlockConfig;

public abstract class InputModule : MonoBehaviour, IInput
{
    [Header("Input Direction")]
    [SerializeField] private Direction inputDirection = Direction.None;

    public event Action<Direction> OnInputDirectionChanged;

    public void SetInputDirection(Direction dir)
    {
        inputDirection = dir;
        OnInputDirectionChanged?.Invoke(dir);
    }

    public Direction GetInputDirection() => inputDirection;

    public virtual bool CanReceive(Direction dir) => dir == inputDirection;
    public abstract bool TryReceive(int amount, Direction dir);
}
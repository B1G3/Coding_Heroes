using System;
using UnityEngine;
using static BlockConfig;

public abstract class OutputModule : MonoBehaviour, IOutput
{
    [Header("Output Direction")]
    [SerializeField] private Direction outputDirection = Direction.None;

    public event Action<Direction> OnOutputDirectionChanged;

    public void SetOutputDirection(Direction dir)
    {
        outputDirection = dir;
        OnOutputDirectionChanged?.Invoke(dir);
    }

    public Direction GetOutputDirection() => outputDirection;

    public virtual bool CanSend(Direction dir) => dir == outputDirection;
    public virtual bool TrySend(out int amount, Direction dir)
    {
        amount = 1;
        return dir == outputDirection;
    }

    public abstract void RegisterOutputTarget(IInput target, Direction toDir);
}
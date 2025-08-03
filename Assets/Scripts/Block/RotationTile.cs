
using System.Collections.Generic;
using UnityEngine;
using static BlockConfig;

public class RotationTile : IGridNode, IConnectable, ILogicalModule, IInput, IOutput
{
    [SerializeField] private LocalDirection inputDirection = LocalDirection.Back;
    [SerializeField] private LocalDirection outputDirection = LocalDirection.Forward;
    
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }

    [SerializeField] private string _next;
    [SerializeField] private string _prev;
    
    public override void Initialize(Vector3Int gridPos)
    {
        base.Initialize(gridPos);
        name = "Rotation";
    }

    public void ConnectNext(IConnectable next)
    {
        Next = next;
        _next = next.ToString();
    }

    public void ConnectPrev(IConnectable prev)
    {
        Prev = prev;
        _prev = prev.ToString();
    }

    public void DisconnectNext()
    {
        Next = null;
    }

    public void DisconnectPrev()
    {
        Prev = null;
    }
    
    public void OnSignalEnter(List<IUnitState> command)
    {
        (Next as ILogicalModule)?.OnSignalEnter(command);
    }
    
    public WorldDirection GetInputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(inputDirection);
    }
    
    public WorldDirection GetOutputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(outputDirection);
    }
}

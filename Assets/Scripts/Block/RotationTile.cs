
using System.Collections.Generic;
using UnityEngine;

public class RotationTile : IGridNode, IConnectable, ILogicalModule
{
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
}

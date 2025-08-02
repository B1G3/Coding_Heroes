
using System.Collections.Generic;
using UnityEngine;

public class MoveNode : IGridNode, IConnectable, ILogicalModule, IGetData
{
    private IUnitState state;
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }

    [SerializeField] private string next;
    [SerializeField] private string prev;
    private ITarget target;
    
    public override void Initialize(Vector3Int gridPos)
    {
        base.Initialize(gridPos);
        name = "Move";
    }

    public void ConnectNext(IConnectable next)
    {
        Next = next;
        this.next = next.ToString();
    }

    public void ConnectPrev(IConnectable prev)
    {
        Prev = prev;
        this.prev = prev.ToString();
    }

    public void DisconnectNext()
    {
        Next = null;
        next = null;
    }

    public void DisconnectPrev()
    {
        Prev = null;
        prev = null; 
    }
    
    public void OnSignalEnter(List<IUnitState> command)
    {
        command.Add(state);
        (Next as ILogicalModule)?.OnSignalEnter(command);
    }

    public void GetData(ITarget target)
    {
        this.target = target;
        state = new MoveState(this.target);
    }
}

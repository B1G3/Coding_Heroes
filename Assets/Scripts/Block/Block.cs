
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Block : IGridNode, IConnectable, ILogicalModule
{
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }

    [SerializeField] private string _next;
    [SerializeField] private string _prev;
    
    public override void Initialize(Vector3Int gridPos, float rotationY)
    {
        base.Initialize(gridPos, rotationY);
        name = "Block";
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
        _next = null;
    }

    public void DisconnectPrev()
    {
        Prev = null;
        _prev = null; 
    }
    
    public async UniTaskVoid OnSignalEnter(List<IUnitState> signal, List<Gnome> gnomes)
    {
        (Next as ILogicalModule)?.OnSignalEnter(signal, gnomes);
    }
}

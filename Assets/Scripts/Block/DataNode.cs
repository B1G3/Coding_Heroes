using UnityEngine;

public class DataNode : IGridNode, IConnectable
{
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }

    [SerializeField] private string next;
    [SerializeField] private string prev;
    
    [SerializeField] private string target;
    
    public override void Initialize(Vector3Int gridPos)
    {
        base.Initialize(gridPos);
        name = "Target";
        prev = "It is target";
    }

    public void ConnectNext(IConnectable next)
    {
        Next = next;
        this.next = next.ToString();
        (next as IGetData)?.GetData(target);
    }

    public void ConnectPrev(IConnectable prev)
    {
        Debug.Log("Target");
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
}

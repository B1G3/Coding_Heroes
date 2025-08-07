using UnityEngine;
using static BlockConfig;

public class DataCollectorNode : IGridNode, IConnectable, IGetData, IInput
{
    [SerializeField] private LocalDirection inputDirection = LocalDirection.Back;
    
    public IConnectable Next { get; set; } // 항상 IfNode
    public IConnectable Prev { get; set; }
    
    [SerializeField] private string next;
    [SerializeField] private string prev;
    
    private ITarget receivedData;
    
    public override void Initialize(Vector3Int gridPos, float rotationY = 0)
    {
        base.Initialize(gridPos, rotationY);
        int steps = DirectionUtils.StepsFromRotationY(rotationY);
        inputDirection = inputDirection.RotateY(steps);
        name = "DataCollector";
    }

    public void ConnectNext(IConnectable next)
    {
        Next = next;
        this.next = next.ToString();
        
        // 연결되는 즉시 데이터가 있으면 바로 전달
        if (receivedData != null)
        {
            (next as IGetData)?.GetData(receivedData);
        }
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

    // Prev에서 데이터를 받음
    public void GetData(ITarget target)
    {
        receivedData = target;
        
        // 데이터를 받는 즉시 Next에 연결되어 있으면 바로 전달
        (Next as IGetData)?.GetData(receivedData);
    }

    public WorldDirection GetInputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(inputDirection);
    }
    
    public ITarget GetReceivedData()
    {
        return receivedData;
    }
}
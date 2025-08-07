using System;
using UnityEngine;
using static BlockConfig;

public class BadDataNode : IGridNode, IConnectable, IOutput
{
    [SerializeField] private LocalDirection outputDirection = LocalDirection.Forward;
    
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }

    [SerializeField] private string next;
    [SerializeField] private string prev;
    
    // 필터링할 DataContainer 타입
    private Type targetDataType = typeof(BadDataContainer);
    
    public override void Initialize(Vector3Int gridPos, float rotationY)
    {
        base.Initialize(gridPos, rotationY);
        int steps = DirectionUtils.StepsFromRotationY(rotationY);
        outputDirection = outputDirection.RotateY(steps);
        
        name = "BadDataFilter";
        prev = "Filters BadDataContainer";
    }

    public void ConnectNext(IConnectable next)
    {
        Next = next;
        this.next = next?.ToString();
        
        // 다음 노드에 필터 데이터 전달
        (next as IGetDataFilter)?.GetDataFilter(IsTargetData);
    }

    public void ConnectPrev(IConnectable prev)
    {
        Debug.Log("BadDataFilter connected");
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

    public WorldDirection GetOutputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(outputDirection);
    }

    // BadDataContainer인지 확인하는 필터 함수
    public bool IsTargetData(DataContainer dataContainer)
    {
        if (dataContainer == null) return false;
        
        // BadDataContainer 타입인지 확인
        return dataContainer.GetType() == targetDataType || 
               dataContainer is BadDataContainer;
    }

    // 필터 타입 설정 (Inspector나 코드에서 변경 가능)
    public void SetFilterType<T>() where T : DataContainer
    {
        targetDataType = typeof(T);
    }
}
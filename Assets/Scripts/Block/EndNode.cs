using System;
using System.Collections.Generic;
using UnityEngine;

public class EndNode : IGridNode, IConnectable, ILogicalModule
{
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }
    
    [SerializeField] private string _next;
    [SerializeField] private string _prev;
    
    [SerializeField] private GameObject unit;
    private Unit _unitInstance;

    public override void Initialize(Vector3Int gridPos)
    {
        base.Initialize(gridPos);
        _unitInstance ??= unit.GetComponent<Unit>();
        name = "End";
        _next = "It is end";
    }

    public void ConnectNext(IConnectable next)
    {
        Debug.Log("End");
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
        // 유닛이 없으면 스폰
        if (_unitInstance == null)
            _unitInstance = Instantiate(unit, transform.position, Quaternion.identity).GetComponent<Unit>();
        // 유닛에게 “끝까지 왔어요” 알림
        _unitInstance.StartUnitAsync(command);
    }
}
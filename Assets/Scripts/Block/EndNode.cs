using System;
using UnityEngine;

public class EndNode : IGridNode, IConnectable, ILogicalModule
{
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }
    
    [SerializeField] private GameObject unit;
    private Unit _unitInstance;

    // private void Awake()
    // {
    //     _unitInstance = unit.GetComponent<Unit>();
    // }

    public override void Initialize(Vector3Int gridPos)
    {
        base.Initialize(gridPos);
        name = "End";
    }

    public void ConnectNext(IConnectable next)
    {
        Next = next;
    }

    public void ConnectPrev(IConnectable prev)
    {
        Prev = prev;
    }

    public void DisconnectNext()
    {
        Next = null;
    }

    public void DisconnectPrev()
    {
        Prev = null;
    }

    public void OnSignalEnter(string signal)
    {
        // 유닛이 없으면 스폰
        if (_unitInstance == null)
            _unitInstance = Instantiate(unit, transform.position, Quaternion.identity).GetComponent<Unit>();
        // 유닛에게 “끝까지 왔어요” 알림
        _unitInstance.StartUnit($"{signal} {name}");
    }
}
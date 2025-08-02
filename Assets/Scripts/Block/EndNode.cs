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
        var go = Instantiate(unit, transform.position, Quaternion.identity);
        go.transform.localScale *= 0.01f;
        _unitInstance = go.GetComponent<Unit>();
        
        // 유닛이 없으면 스폰
        // if (_unitInstance == null)
        // {
        //     // Instantiate 하고
        //     var go = Instantiate(unit, transform.position, Quaternion.identity);
        //
        //     // 스케일을 0.01로 줄여주고
        //     // go.transform.localScale = Vector3.one * 0.01f;
        //
        //     // Unit 컴포넌트 획득
        //     _unitInstance = go.GetComponent<Unit>();
        // }
        // 유닛에게 “끝까지 왔어요” 알림
        _unitInstance.OnFinishCommand += Finish;
        _unitInstance.StartUnitAsync(command);
    }

    private void Finish()
    {
        _unitInstance = null;
    }
}
using System.Collections.Generic;
using UnityEngine;
using static BlockConfig;

public class StartNode : IGridNode, IConnectable, ILogicalModule, IOutput
{
    [SerializeField] private LocalDirection outputDirection = LocalDirection.Forward;
    
    private IUnitState state;
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }
    
    [SerializeField] private string _next;
    [SerializeField] private string _prev;

    public override void Initialize(Vector3Int gridPos)
    {
        base.Initialize(gridPos);
        state = new IdleState();
        name = "Start";
        _prev = "It is start";
    }

    public void ConnectNext(IConnectable next)
    {
        Next = next;
        _next = next.ToString();
    }

    public void ConnectPrev(IConnectable prev)
    {
        Debug.Log("Start");
    }

    public void DisconnectNext()
    {
        Next = null;
    }

    public void DisconnectPrev()
    {
        Prev = null;
    }

    // 외부에서 호출하면 신호 전파를 시작
    public void Launch()
    {
        OnSignalEnter();
    }

    // 신호가 도착했을 때(자기 자신에게), 즉시 Next로 이어줌
    public void OnSignalEnter(List<IUnitState> command = null)
    {
        // 예: 애니메이션 등 효과를 먼저 실행해도 좋습니다.
        (Next as ILogicalModule)?.OnSignalEnter(new List<IUnitState> { state });
    }
    
    public WorldDirection GetOutputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(outputDirection);
    }
}
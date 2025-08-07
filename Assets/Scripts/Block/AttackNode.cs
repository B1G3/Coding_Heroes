using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static BlockConfig;

public class AttackNode : IGridNode, IConnectable, ILogicalModule, IGetData, IOutput, IInput
{
    [SerializeField] private LocalDirection inputDirection = LocalDirection.Back;
    [SerializeField] private LocalDirection outputDirection = LocalDirection.Forward;
    
    private IUnitState state;
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }

    [SerializeField] private string next;
    [SerializeField] private string prev;
    private ITarget target; // If 노드에서 받은 특정 타겟
    
    private List<Gnome> currentGnomes = new List<Gnome>();
    
    public override void Initialize(Vector3Int gridPos, float rotationY = 0)
    {
        base.Initialize(gridPos, rotationY);
        int steps = DirectionUtils.StepsFromRotationY(rotationY);
        inputDirection = inputDirection.RotateY(steps);
        outputDirection = outputDirection.RotateY(steps);
        
        // 기본적으로는 아무 적이나 공격하는 상태로 초기화
        state = new AttackState(null, 1);
        name = "Attack";
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
    
    public async UniTaskVoid OnSignalEnter(List<IUnitState> command, List<Gnome> gnomes)
    {
        currentGnomes = gnomes;
        
        // 공격 상태 추가
        command.Add(state);
        
        // 잠시 대기 (공격 시간)
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.1f));
        
        // 다음 노드로 신호 전파
        (Next as ILogicalModule)?.OnSignalEnter(command, currentGnomes).Forget();
    }

    public void GetData(ITarget target)
    {
        this.target = target;
        
        // 특정 타겟이 지정된 경우 (If 노드에서 온 경우)
        if (target != null && target is IAttackable attackable)
        {
            state = new AttackState(attackable, 1);
        }
        else
        {
            // 타겟이 없으면 아무 적이나 공격
            state = new AttackState(null, 1);
        }
    }
    
    public WorldDirection GetInputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(inputDirection);
    }
    
    public WorldDirection GetOutputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(outputDirection);
    }
}
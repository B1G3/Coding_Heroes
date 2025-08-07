using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static BlockConfig;

public class IfNode : IGridNode, IConnectable, ILogicalModule, IOutput, IInput, IGetData
{
    [SerializeField] private LocalDirection inputDirection = LocalDirection.Back;
    [SerializeField] private LocalDirection outputDirection = LocalDirection.Forward;
    [SerializeField] private LocalDirection falseOutputDirection = LocalDirection.Right;
    
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }
    public IConnectable FalseNext { get; set; }
    
    [SerializeField] private string next;
    [SerializeField] private string prev;
    [SerializeField] private string falseNext;
    
    private ITarget targetData;
    private List<Gnome> currentGnomes = new List<Gnome>();
    
    public override void Initialize(Vector3Int gridPos, float rotationY = 0)
    {
        base.Initialize(gridPos, rotationY);
        int steps = DirectionUtils.StepsFromRotationY(rotationY);
        inputDirection = inputDirection.RotateY(steps);
        outputDirection = outputDirection.RotateY(steps);
        falseOutputDirection = falseOutputDirection.RotateY(steps);
        
        name = "If";
    }

    public void ConnectNext(IConnectable next)
    {
        Next = next;
        this.next = next.ToString();
    }
    
    public void ConnectFalseNext(IConnectable falseNext)
    {
        FalseNext = falseNext;
        this.falseNext = falseNext.ToString();
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

    public void GetData(ITarget target)
    {
        targetData = target;
    }

    public async UniTaskVoid OnSignalEnter(List<IUnitState> command, List<Gnome> gnomes)
    {
        currentGnomes = gnomes;
        
        // 조건 검사를 위한 IfState 생성 후 command에 추가
        System.Func<Unit, bool> condition = (unit) => CheckIfCondition(unit);
        var ifState = new IfState(condition, new List<IUnitState>(), new List<IUnitState>());
        command.Add(ifState);
        
        // 노움들을 Next 노드 위치로 이동 (일단 True 분기로)
        if (Next != null)
        {
            Vector3 nextPosition = (Next as MonoBehaviour).transform.position;
            foreach (var gnome in currentGnomes)
            {
                gnome.SetTarget(nextPosition);
                await UniTask.Delay(System.TimeSpan.FromSeconds(0.3f));
            }
            await WaitForGnomesToReachNext();
        }
        
        // 다음 노드로 신호 전파
        (Next as ILogicalModule)?.OnSignalEnter(command, currentGnomes).Forget();
    }
    
    private bool CheckIfCondition(Unit unit)
    {
        if (targetData != null)
        {
            float distance = Vector3.Distance(unit.transform.position, targetData.Position);
            return distance < 5f;
        }
        
        return false;
    }
    
    private async UniTask WaitForGnomesToReachNext()
    {
        while (true)
        {
            bool allReached = true;
            foreach (var gnome in currentGnomes)
            {
                if (gnome != null && gnome.IsMoving)
                {
                    allReached = false;
                    break;
                }
            }
            
            if (allReached) break;
            await UniTask.Yield();
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
    
    public WorldDirection GetFalseOutputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(falseOutputDirection);
    }
}
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FunctionEndNode : IGridNode, IConnectable, ILogicalModule, IInput
{
    [SerializeField] private BlockConfig.LocalDirection inputDirection = BlockConfig.LocalDirection.Back;
    
    public IConnectable Next { get; set; } // 사용 안함
    public IConnectable Prev { get; set; }
    
    [SerializeField] private string next;
    [SerializeField] private string prev;
    
    public FunctionStartNode ConnectedFunctionStart { get; set; }
    
    public override void Initialize(Vector3Int gridPos, float rotationY = 0)
    {
        base.Initialize(gridPos, rotationY);
        int steps = DirectionUtils.StepsFromRotationY(rotationY);
        inputDirection = inputDirection.RotateY(steps);
        name = "FunctionEnd";
    }
    
    // 🎯 IConnectable 구현
    public void ConnectNext(IConnectable next)
    {
        Next = next; // 함수에서는 실제로 사용 안함
        this.next = next?.ToString();
    }

    public void ConnectPrev(IConnectable prev)
    {
        Prev = prev;
        this.prev = prev?.ToString();
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
    
    // 🎯 IInput 구현
    public BlockConfig.WorldDirection GetInputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(inputDirection);
    }

    // 🎯 ILogicalModule 구현
    public async UniTaskVoid OnSignalEnter(List<IUnitState> command, List<Gnome> gnomes)
    {
        Debug.Log("FunctionEndNode: 함수 실행 완료 - 호출자로 복귀");
        
        // 연결된 함수 시작을 통해 원래 함수 노드로 돌아가기
        if (ConnectedFunctionStart?.Caller != null)
        {
            ConnectedFunctionStart.Caller.OnFunctionComplete(command, gnomes).Forget();
        }
        else
        {
            Debug.LogError("FunctionEndNode: 연결된 함수 시작 노드를 찾을 수 없습니다!");
        }
    }
}
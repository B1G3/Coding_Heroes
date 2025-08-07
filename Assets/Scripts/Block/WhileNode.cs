using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static BlockConfig;

public class WhileNode : IGridNode, IConnectable, ILogicalModule, IGetDataFilter, IInput, IOutput
{
    [SerializeField] private LocalDirection inputDirection = LocalDirection.Back;
    [SerializeField] private LocalDirection outputDirection = LocalDirection.Forward;
    
    [Header("While Settings")]
    [SerializeField] private int maxIterations = 2; // 최대 반복 횟수
    
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }

    [SerializeField] private string next;
    [SerializeField] private string prev;
    
    private List<Gnome> currentGnomes = new List<Gnome>();
    private Func<DataContainer, bool> dataFilter;
    
    public override void Initialize(Vector3Int gridPos, float rotationY = 0)
    {
        base.Initialize(gridPos, rotationY);
        int steps = DirectionUtils.StepsFromRotationY(rotationY);
        inputDirection = inputDirection.RotateY(steps);
        outputDirection = outputDirection.RotateY(steps);
        name = $"While({maxIterations})";
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
    
    public void GetDataFilter(Func<DataContainer, bool> filter)
    {
        dataFilter = filter;
    }
    
    public async UniTaskVoid OnSignalEnter(List<IUnitState> command, List<Gnome> gnomes)
    {
        currentGnomes = gnomes;
        
        // WhileState 생성
        var whileState = new WhileState(maxIterations);
        if (dataFilter != null)
        {
            whileState.SetDataFilter(dataFilter);
        }
        
        command.Add(whileState);
        
        // 노움들 이동은 While이 끝난 후
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
        
        // 다음 노드로 신호 전파 (While 완료 후)
        (Next as ILogicalModule)?.OnSignalEnter(command, currentGnomes).Forget();
    }
    
    private List<IUnitState> CollectLoopStates()
    {
        // 실제로는 뒤에 연결된 노드들을 순회해서 State들을 수집
        // 지금은 템플릿으로 기본 State들 생성
        return new List<IUnitState>
        {
            new MoveState(null, 0f), // 타겟은 런타임에 설정됨
            new AttackState(null, 1)  // 타겟은 런타임에 설정됨
        };
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
}
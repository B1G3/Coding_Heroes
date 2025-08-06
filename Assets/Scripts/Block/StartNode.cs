using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static BlockConfig;

public class StartNode : IGridNode, IConnectable, ILogicalModule, IOutput
{
    [SerializeField] private LocalDirection outputDirection = LocalDirection.Forward;
    
    private IUnitState idleState;
    private IUnitState moveState;
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }
    
    private ITarget target;
    
    [SerializeField] private string _next;
    [SerializeField] private string _prev;
    
    private List<Gnome> spawnedGnomes = new List<Gnome>();

    public UnityEvent onSignalEnterEvent;

    public override void Initialize(Vector3Int gridPos, float rotationY = 0)
    {
        base.Initialize(gridPos, rotationY);
        int steps = DirectionUtils.StepsFromRotationY(rotationY);
        outputDirection = outputDirection.RotateY(steps);
        target = GameManager.Instance.GetTarget();
        idleState = new IdleState();
        moveState = new MoveState(target, 0.75f);
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
        OnSignalEnter().Forget();
    }

    // 신호가 도착했을 때(자기 자신에게), 즉시 Next로 이어줌
    public async UniTaskVoid OnSignalEnter(List<IUnitState> command = null, List<Gnome> gnomes = null)
    {
        SpawnGnomes().Forget();
        await WaitForGnomesToReachNext();

        // 예: 애니메이션 등 효과를 먼저 실행해도 좋습니다.
        (Next as ILogicalModule)?.OnSignalEnter(new List<IUnitState> { idleState, moveState }, spawnedGnomes).Forget();
    }
    
    public WorldDirection GetOutputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(outputDirection);
    }
    
    private async UniTask SpawnGnomes()
    {
        spawnedGnomes.Clear();
        
        Vector3 nextPos = (Next as MonoBehaviour).transform.position;
        Vector3 spawnPosition = transform.position;
        
        for (int i = 0; i < 3; i++)
        {
            onSignalEnterEvent?.Invoke();
            
            GameObject gnome = GnomePool.Instance.Get();
            gnome.transform.position = spawnPosition; // 옆으로 간격을 두고 배치
            
            var gnomeCmp = gnome.GetComponent<Gnome>();
            spawnedGnomes.Add(gnomeCmp);
            gnomeCmp.SetTarget(nextPos);
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        }
    }
    
    private async UniTask WaitForGnomesToReachNext()
    {
        while (true)
        {
            bool allReached = true;
            foreach (var gnome in spawnedGnomes)
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

}

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static BlockConfig;

public class MoveNode : IGridNode, IConnectable, ILogicalModule, IGetData, IOutput, IInput
{
    [SerializeField] private LocalDirection inputDirection = LocalDirection.Back;
    [SerializeField] private LocalDirection outputDirection = LocalDirection.Forward;
    
    private IUnitState state;
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }

    [SerializeField] private string next;
    [SerializeField] private string prev;
    private ITarget target;
    
    private List<Gnome> currentGnomes = new List<Gnome>();
    
    public override void Initialize(Vector3Int gridPos, float rotationY = 0)
    {
        base.Initialize(gridPos, rotationY);
        int steps = DirectionUtils.StepsFromRotationY(rotationY);
        inputDirection = inputDirection.RotateY(steps);
        outputDirection = outputDirection.RotateY(steps);
        name = "Move";
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
        Vector3 nextPosition = (Next as MonoBehaviour).transform.position;
        foreach (var g in currentGnomes)
        {
            g.SetTarget(nextPosition);
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        }

        await WaitForGnomesToReachNext();

        command.Add(state);
        (Next as ILogicalModule)?.OnSignalEnter(command, currentGnomes).Forget();
    }

    public void GetData(ITarget target)
    {
        this.target = target;
        state = new MoveState(this.target, 0f);
    }
    
    public WorldDirection GetInputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(inputDirection);
    }
    
    public WorldDirection GetOutputDirection()
    {
        return DirectionUtils.LocalToWorldDirection(outputDirection);
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

}


using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static BlockConfig;

public class RotationTile : IGridNode, IConnectable, ILogicalModule, IInput, IOutput
{
    [SerializeField] private LocalDirection inputDirection = LocalDirection.Back;
    [SerializeField] private LocalDirection outputDirection = LocalDirection.Forward;
    
    public IConnectable Next { get; set; }
    public IConnectable Prev { get; set; }

    [SerializeField] private string _next;
    [SerializeField] private string _prev;
    
    private List<Gnome> currentGnomes = new List<Gnome>();
    
    public override void Initialize(Vector3Int gridPos, float rotationY = 0)
    {
        base.Initialize(gridPos, rotationY);
        int steps = DirectionUtils.StepsFromRotationY(rotationY);
        inputDirection = inputDirection.RotateY(steps);
        outputDirection = outputDirection.RotateY(steps);
        name = "Rotation";
    }

    public void ConnectNext(IConnectable next)
    {
        Next = next;
        _next = next.ToString();
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
        
        (Next as ILogicalModule)?.OnSignalEnter(command, currentGnomes).Forget();
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

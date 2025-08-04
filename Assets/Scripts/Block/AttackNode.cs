using System.Collections.Generic;
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
    private ITarget target;
    
    public override void Initialize(Vector3Int gridPos, float rotationY)
    {
        base.Initialize(gridPos, rotationY);
        int steps = DirectionUtils.StepsFromRotationY(rotationY);
        inputDirection = inputDirection.RotateY(steps);
        outputDirection = outputDirection.RotateY(steps);
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
    
    public void OnSignalEnter(List<IUnitState> command)
    {
        if (state == null)
            state = new AttackState();
        command.Add(state);
        (Next as ILogicalModule)?.OnSignalEnter(command);
    }

    public void GetData(ITarget target)
    {
        this.target = target;
        state = new AttackState(target as IAttackable);
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
